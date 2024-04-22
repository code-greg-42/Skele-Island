using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    public List<GameObject> pooledEnemies;
    public GameObject enemyToPool;
    public int amountToPool;

    [HideInInspector]
    public bool isPoolLoaded; // flag for other scripts to see when the pool has been fully loaded

    private void Awake()
    {
        // initialize singleton instance
        Instance = this;
    }

    void Start()
    {
        // initialize object pool with inactive enemy objects
        pooledEnemies = new List<GameObject>();
        GameObject enemy;
        for (int i = 0; i < amountToPool; i++)
        {
            enemy = Instantiate(enemyToPool);
            enemy.SetActive(false);
            pooledEnemies.Add(enemy);
        }

        // set flag to true once all enemies are instantiated and added to the pool
        isPoolLoaded = true;
    }

    public GameObject GetPooledEnemy()
    {
        // loop through and return the first inactive enemy
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledEnemies[i].activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }
        return null;
    }

    public bool IsAnyEnemyActive()
    {
        // loop through and return true if any enemy is active
        foreach (GameObject enemy in pooledEnemies)
        {
            if (enemy.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }
}
