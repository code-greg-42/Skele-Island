using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSword : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Enemy enemy = GetComponentInParent<Enemy>();

            if (enemy != null && enemy.isAttacking && enemy.isMidSwing)
            {
                Debug.Log("player was hit!");
                enemy.playerHit = true;
            }
        }
    }
}
