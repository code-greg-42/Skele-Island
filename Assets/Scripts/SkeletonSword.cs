using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSword : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // if the sword collides with the player while the enemy is in mid-swing of its attack sequence, set the playerHit bool to true
        if (other.CompareTag("Player"))
        {
            Enemy enemy = GetComponentInParent<Enemy>();

            if (enemy != null && enemy.isAttacking && enemy.isMidSwing)
            {
                enemy.playerHit = true;
            }
        }
    }
}
