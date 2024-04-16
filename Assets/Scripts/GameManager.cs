using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int enemiesPerWave = 5;
    int waveNumber;

    readonly int finalWave = 5;
    readonly float enemySpeedMin = 3.5f;
    readonly float enemySpeedMax = 10.5f;
    readonly float spawnBoundary = 36.0f;

    // Update is called once per frame
    void Update()
    {
        if (waveNumber <= finalWave)
        {
            if (EnemyPool.Instance.isPoolLoaded && !EnemyPool.Instance.IsAnyEnemyActive())
            {
                waveNumber++;
                if (waveNumber <= finalWave)
                {
                    UIManager.Instance.UpdateWaveNumber(waveNumber, finalWave);
                    enemiesPerWave *= 2;
                    SpawnNewWave(enemiesPerWave);
                }
                else
                {
                    Debug.Log("Game Over!");
                }
            }
        }
    }

    void SpawnNewWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = EnemyPool.Instance.GetPooledEnemy();
            if (enemy != null)
            {
                if (enemy.TryGetComponent<Enemy>(out var enemyScript))
                {
                    float randomSpeed = Random.Range(enemySpeedMin, enemySpeedMax);
                    // apply random speed and acceleration to enemy navmesh
                    enemyScript.navMeshAgent.speed = randomSpeed;
                    enemyScript.navMeshAgent.acceleration = randomSpeed * 1.5f;
                }
                // spawn enemy at random unobstructed point
                enemy.transform.position = GetRandomSpawn();
                enemy.SetActive(true);
            }
        }
    }

    Vector3 GetRandomSpawn()
    {
        float spawnPosX = Random.Range(-spawnBoundary, spawnBoundary);
        float spawnPosZ = Random.Range(-spawnBoundary, spawnBoundary);
        Vector3 spawnPos = new(spawnPosX, 0, spawnPosZ);

        if (!IsSpawnObstructed(spawnPos))
        {
            return spawnPos;
        }
        else
        {
            return GetRandomSpawn();
        }
    }

    bool IsSpawnObstructed(Vector3 spawn)
    {
        Collider[] colliders = Physics.OverlapSphere(spawn, 5.0f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }
}
