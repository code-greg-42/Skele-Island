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
    public bool isPoolLoaded;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledEnemies = new List<GameObject>();
        GameObject enemy;
        for (int i = 0; i < amountToPool; i++)
        {
            enemy = Instantiate(enemyToPool);
            enemy.SetActive(false);
            pooledEnemies.Add(enemy);
        }
        isPoolLoaded = true;
    }

    public GameObject GetPooledEnemy()
    {
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
