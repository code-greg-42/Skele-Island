using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f;
    public float maxHealth = 100.0f;

    [HideInInspector]
    public bool isDying;

    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameManager gameManager;

    readonly float deathAnimationTime = 2.08375f;

    public void TakeDamage(float damage)
    {
        Debug.Log("player took damage!");
        health -= damage;

        if (health <= 0)
        {
            UIManager.Instance.UpdatePlayerHealthBar(0);
            StartCoroutine(DeathCoroutine());
        }
        else
        {
            UIManager.Instance.UpdatePlayerHealthBar(health / maxHealth);
            if (!playerAttack.isCasting && !playerMovement.isRolling)
            {
                anim.SetTrigger("takeDamage");
            }
        }
    }

    IEnumerator DeathCoroutine()
    {
        // set bool to stop movement and play animation
        isDying = true;
        anim.SetTrigger("die");

        // wait for animation to end and end game
        yield return new WaitForSeconds(deathAnimationTime);
        gameManager.EndGame();
    }
}
