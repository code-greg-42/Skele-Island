using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool
{
    // INHERITANCE and POLYMORPHISM
    public static EnemyPool Instance;

    [HideInInspector]
    public bool isPoolLoaded; // flag for other scripts to see when the pool has been fully loaded

    protected override void Awake()
    {
        // initialize singleton instance
        Instance = this;
        base.Awake();
        isPoolLoaded = true;
    }

    // POLYMORPHISM
    public bool IsAnyEnemyActive()
    {
        // loop through and return true if any enemy is active
        foreach (GameObject enemy in pooledObjects)
        {
            if (enemy.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }
}
