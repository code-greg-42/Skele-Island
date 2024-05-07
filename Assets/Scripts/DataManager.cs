using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private int bestWinTime = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ENCAPSULATION

    // public method to set a new best win time if it's less than current best
    public void SetBestWinTime(int newTime)
    {
        if (bestWinTime == 0 || newTime < bestWinTime)
        {
            bestWinTime = newTime;
        }
    }

    // public getter to access the best win time
    public int GetBestWinTime()
    {
        return bestWinTime;
    }
}
