using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f;
    public float maxHealth = 100.0f;

    [Header("References")]
    public Animator anim;
    public PlayerAttack playerAttack;
    public PlayerMovement playerMovement;

    public void TakeDamage(float damage)
    {
        Debug.Log("player took damage!");
        health -= damage;

        if (!playerAttack.isCasting && !playerMovement.isRolling)
        {
            anim.SetTrigger("takeDamage");
        }
    }
}
