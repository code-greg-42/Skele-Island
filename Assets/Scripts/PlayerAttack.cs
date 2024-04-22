using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Variables")]
    public float attackForce;
    public float attackDamage = 50.0f;

    // private attack variables
    readonly float castTime = 0.15f;
    readonly float attackAnimationTime = 0.54882352941f;
    float buttonHoldTime;
    bool attackReady = true;

    [Header("Force Pull Variables")]
    public float forcePullRange;
    public float forcePullSpeed;
    public float forcePullCenterRadius;
    readonly float forcePullCooldown = 3.0f;
    bool forcePullReady = true;

    // variables used in other scripts
    [HideInInspector]
    public int forcePullCharges = 1;
    [HideInInspector]
    public int damageBuffCharges = 1;
    [HideInInspector]
    public bool isCasting;
    [HideInInspector]
    public bool damageBuffAnimationActive;

    // damage buff variables
    readonly float damageBuffMultiplication = 4.0f;
    readonly float damageBuffDuration = 5.0f;
    readonly float damageBuffAnimationTime = 1.18f;
    bool damageBuffReady = true;

    [Header("Keybinds")]
    public KeyCode forcePullKey = KeyCode.C;
    public KeyCode damageBuffKey = KeyCode.E;

    [Header("Projectile Size")]
    public float minProjectileSize = 1f;
    public float maxProjectileSize = 2f;
    public float projectileScaleFactor = 0.5f;

    // attribute maximums
    [HideInInspector]
    public float attackDamageMax = 150.0f;
    [HideInInspector]
    public float minProjectileSizeMax = 0.8f;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject forcePullEffectPrefab;
    [SerializeField] Transform shootFromPoint;
    [SerializeField] Camera mainCam;
    [SerializeField] Animator playerAnim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerHealth playerHealth;

    void Update()
    {
        if (playerMovement.grounded && !damageBuffAnimationActive && !playerHealth.isDying && attackReady)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                isCasting = true;
                playerAnim.SetBool("isCasting", true);
            }

            if (Input.GetButton("Fire1"))
            {
                isCasting = true;
                playerAnim.SetBool("isCasting", true);
                buttonHoldTime += Time.deltaTime;

                // update castbar UI component
                if (buttonHoldTime <= castTime)
                {
                    UIManager.Instance.UpdateCastBar(buttonHoldTime / castTime);

                    if (!UIManager.Instance.IsCastBarActive())
                    {
                        UIManager.Instance.ActivateCastBar();
                    }
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (buttonHoldTime > castTime)
                {
                    Shoot(buttonHoldTime);
                }
                UIManager.Instance.DeactivateCastBar();
                attackReady = false;
                isCasting = false;
                playerAnim.SetBool("isCasting", false);
                buttonHoldTime = 0f;
                StartCoroutine(ResetMainAttack());
            }
        }
        
        if (Input.GetKeyDown(forcePullKey) && forcePullReady && forcePullCharges > 0 && !playerHealth.isDying)
        {
            forcePullReady = false;

            forcePullCharges -= 1;
            UIManager.Instance.UpdateForcePullCharges(forcePullCharges);

            ForcePull();
            StartCoroutine(ResetForcePull());
        }

        if (Input.GetKeyDown(damageBuffKey) && damageBuffReady && playerMovement.grounded && !isCasting && !playerMovement.isRolling && damageBuffCharges > 0 && !playerHealth.isDying)
        {
            damageBuffReady = false;

            damageBuffCharges -= 1;
            UIManager.Instance.UpdateDamageBuffCharges(damageBuffCharges);

            StartCoroutine(DamageBuffCoroutine());
        }
    }

    void Shoot(float holdTime)
    {
        playerAnim.SetTrigger("attack");

        float projectileSize = Mathf.Clamp(minProjectileSize + holdTime * projectileScaleFactor, minProjectileSize, maxProjectileSize);
        Debug.Log(projectileSize);
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // if raycast doesn't hit, shoot far away
            targetPoint = ray.GetPoint(1000); // arbitrary distance
        }

        // calc direction from shootFromPoint to targetPoint
        Vector3 direction = (targetPoint - shootFromPoint.position).normalized;

        // get projectile from object pool
        GameObject projectile = ProjectilePool.Instance.GetPooledProjectile();

        if (projectile != null)
        {
            projectile.transform.localScale = Vector3.one * projectileSize;
            // reposition projectile from pool
            projectile.transform.position = shootFromPoint.position;
            // rotate projectile to face target direction
            projectile.transform.forward = direction;
            // activate projectile in hierarchy
            projectile.SetActive(true);

            
            if (projectile.TryGetComponent<DeactivateProjectile>(out var projectileScript))
            {
                // set origin of projectile for max distance functionality
                projectileScript.SetOrigin(shootFromPoint.position);

                // set attack damage of projectile
                projectileScript.attackDamage = attackDamage;
            }

            // apply velocity
            if (projectile.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = direction * attackForce;
            }
        }
    }

    void ForcePull()
    {
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        Vector3 pullPosition;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("whatIsGround")))
        {
            pullPosition = hit.point;
        }
        else
        {
            return;
        }

        // instantiate force pull effect
        Instantiate(forcePullEffectPrefab, pullPosition, forcePullEffectPrefab.transform.rotation);

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy != null && !enemy.isBoss && Vector3.Distance(enemy.transform.position, pullPosition) <= forcePullRange)
            {
                StartCoroutine(MoveEnemyToTarget(enemy, pullPosition));
            }
        }
    }

    IEnumerator MoveEnemyToTarget(Enemy enemy, Vector3 targetPosition)
    {
        // set bool and method to stop movement from interfering
        enemy.isBeingPulled = true;
        enemy.StopMoving();
        enemy.navMeshAgent.enabled = false;

        while (Vector3.Distance(targetPosition, enemy.transform.position) > forcePullCenterRadius)
        {
            Vector3 targetDirection = (targetPosition - enemy.transform.position).normalized;
            enemy.transform.position += forcePullSpeed * Time.deltaTime * targetDirection;

            yield return null;
        }

        // reset bool to allow for continued movement
        enemy.navMeshAgent.enabled = true;
        enemy.isBeingPulled = false;
    }

    IEnumerator ResetForcePull()
    {
        yield return new WaitForSeconds(forcePullCooldown);
        forcePullReady = true;
    }

    IEnumerator ResetMainAttack()
    {
        yield return new WaitForSeconds(attackAnimationTime);
        attackReady = true;
    }

    IEnumerator DamageBuffCoroutine()
    {
        // start animation and set bool
        playerAnim.SetTrigger("buffSelf");
        damageBuffAnimationActive = true;

        // save original attack damage value to reassign at end
        float originalAttackDamage = attackDamage;

        // increase attack damage by buff multiplication amount
        attackDamage *= damageBuffMultiplication;

        // wait for animation to end and set bool to re-allow casting
        yield return new WaitForSeconds(damageBuffAnimationTime);
        damageBuffAnimationActive = false;

        // wait for duration time and reset bool and original attack damage
        yield return new WaitForSeconds(damageBuffDuration);
        attackDamage = originalAttackDamage;
        damageBuffReady = true;
    }
}
