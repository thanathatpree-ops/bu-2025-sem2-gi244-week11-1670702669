using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public int totalSpawnEnemies;
    public int numberOfRandomSpawnPoint;
    public float delayStart;
    public float spawnInterval;
    public int numberOfPowerUp;
}

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Configuration")]
    public Wave[] waves;

    private int currentWaveIndex = 0;

    void Start()
    {
        if (waves == null || waves.Length == 0)
        {
            waves = new Wave[]
            {
                new Wave { totalSpawnEnemies = 4,  numberOfRandomSpawnPoint = 1, delayStart = 2f, spawnInterval = 2.0f, numberOfPowerUp = 0 },
                new Wave { totalSpawnEnemies = 6,  numberOfRandomSpawnPoint = 2, delayStart = 2f, spawnInterval = 2.0f, numberOfPowerUp = 1 },
                new Wave { totalSpawnEnemies = 8,  numberOfRandomSpawnPoint = 4, delayStart = 2f, spawnInterval = 2.0f, numberOfPowerUp = 1 },
                new Wave { totalSpawnEnemies = 10, numberOfRandomSpawnPoint = 6, delayStart = 5f, spawnInterval = 0.5f, numberOfPowerUp = 2 },
            };
        }

        StartCoroutine(RunAllWaves());
    }

    IEnumerator RunAllWaves()
    {
        Debug.Log($"[SpawnManager] เริ่มเกม — มีทั้งหมด {waves.Length} Wave");

        for (int i = 0; i < waves.Length; i++)
        {
            currentWaveIndex = i;
            Debug.Log($"[SpawnManager] ===== Wave {i + 1} / {waves.Length} เริ่มแล้ว =====");
            yield return StartCoroutine(RunWave(waves[i]));
            Debug.Log($"[SpawnManager] ===== Wave {i + 1} จบแล้ว =====");
        }

        Debug.Log("[SpawnManager] ครบทุก Wave แล้ว!");
    }

    IEnumerator RunWave(Wave wave)
    {
        List<Transform> selectedPoints = GetRandomSpawnPoints(wave.numberOfRandomSpawnPoint);
        Debug.Log($"[SpawnManager] เลือก Spawn Points จำนวน {selectedPoints.Count} จุด");

        for (int i = 0; i < wave.numberOfPowerUp; i++)
        {
            if (powerUpPrefab != null)
            {
                Transform pt = selectedPoints[Random.Range(0, selectedPoints.Count)];
                Instantiate(powerUpPrefab, pt.position, Quaternion.identity);
                Debug.Log($"[SpawnManager] Spawn PowerUp {i + 1}/{wave.numberOfPowerUp}");
            }
        }

        Debug.Log($"[SpawnManager] รอ {wave.delayStart} วินาที ก่อน Spawn ศัตรู...");
        yield return new WaitForSeconds(wave.delayStart);

        Debug.Log($"[SpawnManager] เริ่ม Spawn ศัตรู — เป้าหมาย {wave.totalSpawnEnemies} ตัว");
        for (int i = 0; i < wave.totalSpawnEnemies; i++)
        {
            Transform spawnPt = selectedPoints[Random.Range(0, selectedPoints.Count)];
            Instantiate(enemyPrefab, spawnPt.position, Quaternion.identity);
            Debug.Log($"[SpawnManager] Spawn ศัตรู {i + 1}/{wave.totalSpawnEnemies} ที่ {spawnPt.name}");
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        Debug.Log($"[SpawnManager] Spawn ครบ {wave.totalSpawnEnemies} ตัวแล้ว");
    }

    List<Transform> GetRandomSpawnPoints(int count)
    {
        List<Transform> pool = new List<Transform>(spawnPoints);
        List<Transform> selected = new List<Transform>();

        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return selected;
    }
}
