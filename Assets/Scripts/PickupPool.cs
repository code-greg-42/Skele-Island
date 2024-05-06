using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPool : ObjectPool
{
    // INHERITANCE
    public static PickupPool Instance;

    protected override void Awake()
    {
        // initialize singleton instance
        Instance = this;
        // call base awake method
        base.Awake();
    }
}
