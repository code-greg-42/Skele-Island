using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePickup : MonoBehaviour
{
    readonly float lifetime = 12.5f; // determines how long a pickup stays active for after spawning
    float aliveTimer = 0f;

    // buff increase amounts
    readonly float attackDamageIncrease = 5.0f;
    readonly float moveSpeedIncrease = 0.5f;
    readonly float projectileSizeIncrease = 0.1f;

    // script references
    PlayerAttack playerAttack;
    PlayerHealth playerHealth;
    PlayerMovement playerMovement;

    // enum for buff types
    enum BuffType
    {
        MaxHealth,
        IncreaseSpeed,
        IncreaseDamage,
        IncreaseProjectileSize,
        IncreaseForcePullCharge,
        IncreaseDamageBuffCharge
    }

    private void Awake()
    {
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
            playerHealth = player.GetComponent<PlayerHealth>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        aliveTimer += Time.deltaTime;
        if (aliveTimer > lifetime)
        {
            Deactivate();
        }
    }

    void Deactivate()
    {
        aliveTimer = 0f;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyRandomBuff();
            Deactivate();
        }
    }

    void ApplyRandomBuff()
    {
        // get random buff from BuffType enum
        BuffType buff = (BuffType)Random.Range(0, System.Enum.GetValues(typeof(BuffType)).Length);

        string buffMessage = "";

        // apply buff and display message based on randomly selected buff
        switch (buff)
        {
            case BuffType.MaxHealth:
                if (playerHealth.health < playerHealth.maxHealth)
                {
                    playerHealth.health = playerHealth.maxHealth;
                    UIManager.Instance.UpdatePlayerHealthBar(1);
                    buffMessage = "Health Recovered!";
                } else
                {
                    buffMessage = "Health already at max!";
                }
                break;

            case BuffType.IncreaseSpeed:
                if (playerMovement.moveSpeed < playerMovement.moveSpeedMax)
                {
                    playerMovement.moveSpeed += moveSpeedIncrease;
                    buffMessage = "Speed Increased!";
                } else
                {
                    buffMessage = "Move Speed at Maximum!";
                }
                break;

            case BuffType.IncreaseDamage:
                if (playerAttack.attackDamage < playerAttack.attackDamageMax)
                {
                    playerAttack.attackDamage += attackDamageIncrease;
                    buffMessage = "Attack Damage Increased!";
                } else
                {
                    buffMessage = "Attack Damage at Maximum!";
                }
                break;

            case BuffType.IncreaseProjectileSize:
                if (playerAttack.minProjectileSize < playerAttack.minProjectileSizeMax)
                {
                    playerAttack.minProjectileSize += projectileSizeIncrease;
                    buffMessage = "Projectile Size Increased!";
                } else
                {
                    buffMessage = "Projectile Size at Maximum!";
                }
                break;

            case BuffType.IncreaseForcePullCharge:
                playerAttack.forcePullCharges += 1;
                UIManager.Instance.UpdateForcePullCharges(playerAttack.forcePullCharges);
                buffMessage = "Force Pull Charges Increased!";
                break;

            case BuffType.IncreaseDamageBuffCharge:
                playerAttack.damageBuffCharges += 1;
                UIManager.Instance.UpdateDamageBuffCharges(playerAttack.damageBuffCharges);
                buffMessage = "Damage Buff Charges Increased!";
                break;
        }

        // display buff message on UI
        UIManager.Instance.DisplayBuffMessage(buffMessage);
    }
}
