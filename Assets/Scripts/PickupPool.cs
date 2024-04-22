using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPool : MonoBehaviour
{
    public static PickupPool Instance;
    public List<GameObject> pooledPickups;
    public GameObject pickupToPool;
    public int amountToPool;

    private void Awake()
    {
        // initialize singleton instance
        Instance = this;
    }
    
    void Start()
    {
        // initialize object pool with inactive pickup objects
        pooledPickups = new List<GameObject>();
        GameObject pickup;
        for (int i = 0; i < amountToPool; i++)
        {
            pickup = Instantiate(pickupToPool);
            pickup.SetActive(false);
            pooledPickups.Add(pickup);
        }
    }

    public GameObject GetPooledPickup()
    {
        // loop through and return the first inactive pickup object
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledPickups[i].activeInHierarchy)
            {
                return pooledPickups[i];
            }
        }
        return null;
    }
}
