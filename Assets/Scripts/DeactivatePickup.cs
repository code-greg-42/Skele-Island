using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePickup : MonoBehaviour
{
    readonly float lifetime = 15.0f;
    float aliveTimer = 0f;

    readonly float attackDamageIncrease = 5.0f;
    readonly float moveSpeedIncrease = 1.0f;
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
        IncreaseForcePullCharge
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
                    Debug.Log("Health Recovered!");
                    buffMessage = "Health Recovered!";
                } else
                {
                    Debug.Log("Player already full health!");
                    buffMessage = "Health already at max!";
                }
                break;

            case BuffType.IncreaseSpeed:
                if (playerMovement.moveSpeed < playerMovement.moveSpeedMax)
                {
                    playerMovement.moveSpeed += moveSpeedIncrease;
                    Debug.Log("Speed Increased!");
                    buffMessage = "Speed Increased!";
                } else
                {
                    Debug.Log("Move Speed at Maximum!");
                    buffMessage = "Move Speed at Maximum!";
                }
                break;

            case BuffType.IncreaseDamage:
                if (playerAttack.attackDamage < playerAttack.attackDamageMax)
                {
                    playerAttack.attackDamage += attackDamageIncrease;
                    Debug.Log("Attack Damage Increased!");
                    buffMessage = "Attack Damage Increased!";
                } else
                {
                    Debug.Log("Attack Damage at Maximum!");
                    buffMessage = "Attack Damage at Maximum!";
                }
                break;

            case BuffType.IncreaseProjectileSize:
                if (playerAttack.minProjectileSize < playerAttack.minProjectileSizeMax)
                {
                    playerAttack.minProjectileSize += projectileSizeIncrease;
                    Debug.Log("Projectile Size Increased!");
                    buffMessage = "Projectile Size Increased!";
                } else
                {
                    Debug.Log("Projectile Size at Maximum!");
                    buffMessage = "Projectile Size at Maximum!";
                }
                break;

            case BuffType.IncreaseForcePullCharge:
                playerAttack.forcePullCharges += 1;
                UIManager.Instance.UpdateForcePullCharges(playerAttack.forcePullCharges);
                Debug.Log("Force Pull Charges Increased!");
                buffMessage = "Force Pull Charges Increased!";
                break;
        }

        // display buff message on UI
        UIManager.Instance.DisplayBuffMessage(buffMessage);
    }
}
