using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Variables")]
    public float health = 100.0f;
    public float maxHealth = 100.0f;

    [HideInInspector]
    public bool isDying;

    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameManager gameManager;

    readonly float boundary = -15.0f; // boundary for checking if player has fallen off the edge
    readonly float deathAnimationTime = 2.08375f; // time it takes for death animation to play fully

    private void Update()
    {
        // end game if player falls off the edge
        if (gameManager.isGameActive && transform.position.y < boundary)
        {
            gameManager.EndGame();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isDying)
        {
            Debug.Log("player took damage!");
            health -= damage;

            // if health reaches 0, update health bar and start dying sequence
            if (health <= 0)
            {
                UIManager.Instance.UpdatePlayerHealthBar(0);
                StartCoroutine(DeathCoroutine());
            }
            else
            {
                // update health bar to reflect current health
                UIManager.Instance.UpdatePlayerHealthBar(health / maxHealth);

                // trigger take damage animation unless player is casting or rolling
                if (!playerAttack.isCasting && !playerMovement.isRolling)
                {
                    anim.SetTrigger("takeDamage");
                }
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
