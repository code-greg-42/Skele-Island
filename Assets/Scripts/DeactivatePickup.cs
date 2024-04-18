using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePickup : MonoBehaviour
{
    readonly float lifetime = 10.0f;
    float aliveTimer = 0f;

    readonly float attackDamageIncrease = 5.0f;
    readonly float moveSpeedIncrease = 0.5f;
    readonly float projectileSizeIncrease = 0.1f;

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
        BuffType buff = (BuffType)Random.Range(0, System.Enum.GetValues(typeof(BuffType)).Length);
        string buffMessage = "";

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
