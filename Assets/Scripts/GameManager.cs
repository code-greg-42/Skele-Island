using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // wave management variables
    int enemiesPerWave = 5;
    int waveNumber;
    readonly int finalWave = 4;

    [HideInInspector]
    public bool isGameActive;

    // enemy variables
    private readonly float spawnBoundary = 36.0f;
    private readonly float enemySpeedMin = 3.5f;
    private float enemySpeedMax = 10.5f;
    private float enemyAttackDamage = 25.0f;
    private float enemyPickupDropRate = 0.25f;

    // boss variables
    private float bossSpeed = 5.0f;
    private float bossHealth = 8000.0f;
    private readonly float bossSizeIncrease = 2.0f; // constant across difficulties
    private readonly float bossAttackDamage = 100.0f; // constant across difficulties

    // easy difficulty settings
    private readonly float easyEnemyMaxSpeed = 4.0f; // max speed an enemy can roll for its speed attribute
    private readonly float easyEnemyAttackDamage = 10.0f; // attack damage an enemy does with sword attack (player health = 100)
    private readonly float easyEnemyPickupDropRate = 0.35f; // how frequently pickups drop on enemy death
    private readonly float easyBossSpeed = 4.0f;
    private readonly float easyBossHealth = 4000.0f;

    // medium difficulty settings
    private readonly float mediumEnemyMaxSpeed = 8.0f;
    private readonly float mediumEnemyAttackDamage = 25.0f;
    private readonly float mediumEnemyPickupDropRate = 0.25f;
    private readonly float mediumBossSpeed = 5.0f;
    private readonly float mediumBossHealth = 6000.0f;

    // hard difficulty settings
    private readonly float hardEnemyMaxSpeed = 12.0f;
    private readonly float hardEnemyAttackDamage = 50.0f;
    private readonly float hardEnemyPickupDropRate = 0.15f;
    private readonly float hardBossSpeed = 6.0f;
    private readonly float hardBossHealth = 8000.0f;

    private void Start()
    {
        // pause game as game starts with menu launched
        Time.timeScale = 0;
        isGameActive = false;
    }

    void Update()
    {
        if (isGameActive)
        {
            if (EnemyPool.Instance.isPoolLoaded && !EnemyPool.Instance.IsAnyEnemyActive())
            {
                waveNumber++;
                if (waveNumber <= finalWave)
                {
                    // update wave number UI component
                    UIManager.Instance.UpdateWaveNumber(waveNumber, finalWave);

                    // increase number of enemies to spawn and spawn new wave
                    enemiesPerWave *= 2;
                    SpawnNewWave(enemiesPerWave);
                }
                else if (waveNumber == finalWave + 1)
                {
                    // boss wave
                    UIManager.Instance.UpdateWaveNumber(waveNumber, finalWave, true);
                    UIManager.Instance.ActivateBossHealthBar();
                    SpawnBoss();
                }
                else
                {
                    // end game with win = true
                    EndGame(true);
                }
            }
        }
    }

    public void EndGame(bool win = false)
    {
        // set game bool to false
        isGameActive = false;

        // pause the game
        Time.timeScale = 0;

        // unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // bring up win or loss menu based on bool
        if (win)
        {
            UIManager.Instance.UpdateTotalTimeText();
            UIManager.Instance.ActivateWinMenu();
        }
        else
        {
            UIManager.Instance.ActivateGameOverMenu();
        }
    }

    public void RestartGame()
    {
        // reload current scene to replay game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame(int difficulty)
    {
        // set game manager variables according to difficulty
        if (difficulty == 1)
        {
            enemySpeedMax = easyEnemyMaxSpeed;
            enemyAttackDamage = easyEnemyAttackDamage;
            enemyPickupDropRate = easyEnemyPickupDropRate;
            bossSpeed = easyBossSpeed;
            bossHealth = easyBossHealth;
        } else if (difficulty == 2) 
        {
            enemySpeedMax = mediumEnemyMaxSpeed;
            enemyAttackDamage = mediumEnemyAttackDamage;
            enemyPickupDropRate = mediumEnemyPickupDropRate;
            bossSpeed = mediumBossSpeed;
            bossHealth = mediumBossHealth;
        } else if (difficulty == 3)
        {
            enemySpeedMax = hardEnemyMaxSpeed;
            enemyAttackDamage = hardEnemyAttackDamage;
            enemyPickupDropRate = hardEnemyPickupDropRate;
            bossSpeed = hardBossSpeed;
            bossHealth = hardBossHealth;
        }

        // lock cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // bring up in game UI components
        UIManager.Instance.ActivateInGameUI();
        isGameActive = true;

        // unpause game
        Time.timeScale = 1;
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

                    // set attack damage and drop rate accordingly
                    enemyScript.attackDamage = enemyAttackDamage;
                    enemyScript.pickupDropRate = enemyPickupDropRate;
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
                enemyScript.navMeshAgent.speed = bossSpeed;
                enemyScript.navMeshAgent.speed = bossSpeed * 1.5f;

                // set damage, range and health values to boss level
                enemyScript.attackRange *= bossSizeIncrease;
                enemyScript.attackDamage = bossAttackDamage;
                enemyScript.health = bossHealth;
                enemyScript.maxHealth = bossHealth;
            }
            enemy.SetActive(true);
        }
    }

    Vector3 GetRandomSpawn()
    {
        // get random x and z spawn positions
        float spawnPosX = Random.Range(-spawnBoundary, spawnBoundary);
        float spawnPosZ = Random.Range(-spawnBoundary, spawnBoundary);
        Vector3 spawnPos = new(spawnPosX, 0, spawnPosZ);

        if (!IsSpawnObstructed(spawnPos))
        {
            return spawnPos;
        }
        else
        {
            // run method again if obstacle is found at spawnPos
            return GetRandomSpawn();
        }
    }

    bool IsSpawnObstructed(Vector3 spawn)
    {
        // get array of collider objects within sphere near spawn
        Collider[] colliders = Physics.OverlapSphere(spawn, 5.0f);

        // loop through all collider objects and return true if obstacle is found
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
