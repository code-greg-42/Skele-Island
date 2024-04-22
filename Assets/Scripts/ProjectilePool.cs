using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;
    public List<GameObject> pooledProjectiles;
    public GameObject projectileToPool;
    public int amountToPool;

    private void Awake()
    {
        // initialize singleton instance
        Instance = this;
    }

    void Start()
    {
        // initialize object pool with inactive projectile objects
        pooledProjectiles = new List<GameObject>();
        GameObject projectile;
        for (int i = 0; i < amountToPool; i++)
        {
            projectile = Instantiate(projectileToPool);
            projectile.SetActive(false);
            pooledProjectiles.Add(projectile);
        }
    }

    public GameObject GetPooledProjectile()
    {
        // loop through and return the first inactive projectile
        for (int i = 0;i < amountToPool;i++)
        {
            if (!pooledProjectiles[i].activeInHierarchy)
            {
                return pooledProjectiles[i];
            }
        }
        return null;
    }
}
