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
        Instance = this;
    }
    
    void Start()
    {
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
