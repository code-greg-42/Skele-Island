using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int enemiesPerWave = 5;
    int waveNumber;

    readonly int finalWave = 5;
    readonly float enemySpeedMin = 3.5f;
    readonly float enemySpeedMax = 14.0f;
    readonly float spawnBoundary = 36.0f;

    readonly float bossSizeIncrease = 2.0f;
    readonly float bossAttackDamage = 100.0f;
    readonly float bossHealth = 2000.0f;

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
                else if (waveNumber == finalWave + 1)
                {
                    UIManager.Instance.UpdateWaveNumber(waveNumber, finalWave, true);
                    SpawnBoss();
                    Debug.Log("Boss spawned!");
                }
                else
                {
                    EndGame(true);
                    Debug.Log("Game Over");
                }
            }
        }
    }

    public void EndGame(bool win = false)
    {
        Time.timeScale = 0;
        Debug.Log("Game Over");

        if (win)
        {
            UIManager.Instance.ChangeGameResultText("You Win!", win);
        }
        UIManager.Instance.ActivateGameOverMenu();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    void SpawnBoss()
    {
        GameObject enemy = EnemyPool.Instance.GetPooledEnemy();
        if (enemy != null)
        {
            // set position to origin and raise localScale value for boss effect
            enemy.transform.position = Vector3.zero;
            enemy.transform.localScale *= bossSizeIncrease;

            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                // set boss bool to true
                enemyScript.isBoss = true;

                // set min speed to navmeshagent speed value
                enemyScript.navMeshAgent.speed = enemySpeedMin;
                enemyScript.navMeshAgent.speed = enemySpeedMin * 1.5f;

                // set damage, range and health values to boss level
                enemyScript.attackRange *= bossSizeIncrease;
                enemyScript.attackDamage = bossAttackDamage;
                enemyScript.health = bossHealth;
            }
            enemy.SetActive(true);
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
