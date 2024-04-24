using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Attack Variables")]
    public float attackRange = 2.0f;
    public float attackDamage = 10.0f;

    [Header("Health Variables")]
    public float health = 100.0f;
    public float maxHealth = 100.0f;
    public float pickupDropRate = 0.25f;

    [HideInInspector]
    public bool isAttacking; // for preventing other actions while attacking
    [HideInInspector]
    public bool isMidSwing; // for establishing if an attack is in position to hit a player
    [HideInInspector]
    public bool isBeingPulled; // for preventing rogue movements while enemy is under the effect of a force pull
    [HideInInspector]
    public bool playerHit; // for establishing if player has been hit by an attack
    [HideInInspector]
    public bool isBoss; // for changing take damage effect if enemy is a boss

    [Header("References")]
    public ParticleSystem fireParticleSystem;

    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    // private references
    Transform player;
    Animator enemyAnim;

    // animation time variables
    readonly float deathAnimationTime = 2.167f;
    readonly float attackAnimationWindupTime = 0.8f;
    readonly float attackAnimationSwingTime = 1.06f;
    readonly float attackAnimationTotalTime = 2.767f;
    readonly float takeDamageAnimationTime = 0.833f;

    // used for preventing movement while enemy is dying or taking damage
    bool isTakingDamage;
    bool isDying;

    void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player != null && !isDying && !isTakingDamage && !isBeingPulled)
        {
            // check if the player object is within attack range
            if (Vector3.Distance(transform.position, player.position) <= attackRange && !isAttacking)
            {
                StartCoroutine(AttackSequence());
            }
            else
            {
                // if not within attack range, set player as destination
                navMeshAgent.SetDestination(player.position);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // stop particle system if it's already active
        if (fireParticleSystem.isPlaying)
        {
            fireParticleSystem.Stop();
        }

        // play particle system
        fireParticleSystem.Play();

        // if health drops to zero or below, start the death sequence
        if (health <= 0)
        {
            if (isBoss)
            {
                UIManager.Instance.UpdateBossHealthBar(0);
            }
            StartCoroutine(DeathSequence());
        }
        else
        {
            if (!isBoss)
            {
                // if alive and not a boss, play damage animation
                StartCoroutine(TakeDamageSequence());
            }
            else
            {
                // update boss healthbar with current health
                UIManager.Instance.UpdateBossHealthBar(health / maxHealth);
            }
        }
    }

    public void StopMoving()
    {
        // set destination to self to prevent further movement
        navMeshAgent.SetDestination(transform.position);
        // stop any current movement by setting velocity to 0
        navMeshAgent.velocity = Vector3.zero;
    }

    private void DropPickup()
    {
        if (!isBoss)
        {
            // get random value between 0 and 1
            float randomRoll = Random.value;
            if (randomRoll < pickupDropRate)
            {
                // get pickup from pickup pool
                GameObject pickup = PickupPool.Instance.GetPooledPickup();
                if (pickup != null)
                {
                    // drop pickup on dead enemy, using the pickup prefab's y position to ensure correct visibility
                    Vector3 dropPosition = new(transform.position.x, pickup.transform.position.y, transform.position.z);
                    pickup.transform.position = dropPosition;
                    pickup.SetActive(true);
                }
            }
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // rotate towards player and initiate attack animation
        transform.LookAt(player);
        enemyAnim.SetTrigger("attack");

        // wait for windup of animation and set bool to allow determination of player being hit
        yield return new WaitForSeconds(attackAnimationWindupTime);
        isMidSwing = true;

        // wait for swing part of animation to complete and determine if player was hit
        yield return new WaitForSeconds(attackAnimationSwingTime - attackAnimationWindupTime);

        if (playerHit)
        {
            // get root object where scripts are located
            GameObject parentPlayer = player.root.gameObject;

            // look for player health reference
            if (parentPlayer.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                // apply sword damage to player
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // reset player hit determination bools
        isMidSwing = false;
        playerHit = false;

        // wait for rest of animation to end and reset bool to allow additional attacks
        yield return new WaitForSeconds(attackAnimationTotalTime - attackAnimationSwingTime);
        isAttacking = false;
    }

    IEnumerator TakeDamageSequence()
    {
        // stop movement during take damage animation
        isTakingDamage = true;
        StopMoving();

        // play damage animation
        enemyAnim.SetTrigger("takeDamage");

        // wait for animation to complete and reset bool
        yield return new WaitForSeconds(takeDamageAnimationTime);
        isTakingDamage = false;
    }

    IEnumerator DeathSequence()
    {
        // stop navmesh movement
        isDying = true;
        StopMoving();

        // play death animation
        enemyAnim.SetTrigger("die");

        // drop pickup if random roll hits
        DropPickup();

        // wait for death animation time then deactivate
        yield return new WaitForSeconds(deathAnimationTime);

        // reset all bools
        isAttacking = false;
        isMidSwing = false;
        playerHit = false;
        isTakingDamage = false;
        isBeingPulled = false;
        isDying = false;

        // reset health
        health = maxHealth;

        // deactivate and return to pool
        gameObject.SetActive(false);
    }
}
