using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Variables")]
    public float attackForce;
    public float attackDamage = 50.0f;

    readonly float castTime = 0.15f;
    float buttonHoldTime;

    [Header("Force Pull Variables")]
    public float forcePullRange;
    public float forcePullSpeed;
    public float forcePullCenterRadius;
    public int forcePullCharges = 1;
    readonly float forcePullCooldown = 3.0f;
    bool forcePullReady = true;

    [HideInInspector]
    public bool isCasting;
    [HideInInspector]
    public bool damageBuffAnimationActive;
    readonly float damageBuffDuration = 4.0f;
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

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.grounded && !damageBuffAnimationActive)
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
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (buttonHoldTime > castTime)
                {
                    Shoot(buttonHoldTime);
                }
                isCasting = false;
                playerAnim.SetBool("isCasting", false);
                buttonHoldTime = 0f;
            }
        }
        
        if (Input.GetKeyDown(forcePullKey) && forcePullReady && forcePullCharges > 0)
        {
            forcePullReady = false;
            ForcePull();

            forcePullCharges -= 1;
            UIManager.Instance.UpdateForcePullCharges(forcePullCharges);
            StartCoroutine(ResetForcePull());
        }

        if (Input.GetKeyDown(damageBuffKey) && damageBuffReady && playerMovement.grounded && !isCasting && !playerMovement.isRolling)
        {
            damageBuffReady = false;
            StartCoroutine(DamageBuffCoroutine());
        }
    }

    void Shoot(float holdTime)
    {
        playerAnim.SetTrigger("attack");

        float projectileSize = Mathf.Clamp(holdTime * projectileScaleFactor, minProjectileSize, maxProjectileSize);

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
            Debug.Log("Missed target with force pull!");
            return;
        }

        // instantiate force pull effect
        Instantiate(forcePullEffectPrefab, pullPosition, forcePullEffectPrefab.transform.rotation);

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy != null && Vector3.Distance(enemy.transform.position, pullPosition) <= forcePullRange)
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
        enemy.isBeingPulled = false;
        enemy.navMeshAgent.enabled = true;
    }

    IEnumerator ResetForcePull()
    {
        yield return new WaitForSeconds(forcePullCooldown);
        forcePullReady = true;
    }

    IEnumerator DamageBuffCoroutine()
    {
        playerAnim.SetTrigger("buffSelf");
        damageBuffAnimationActive = true;

        Debug.Log("Damage Buff Active!");

        // wait for animation to end
        yield return new WaitForSeconds(damageBuffAnimationTime);
        damageBuffAnimationActive = false;

        // wait for duration time and set bool back to ready
        yield return new WaitForSeconds(damageBuffDuration);

        Debug.Log("Damage Buff Ended!");
        damageBuffReady = true;
    }
}
