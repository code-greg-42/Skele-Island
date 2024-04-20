using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateProjectile : MonoBehaviour
{
    readonly float maxTravelDistance = 75.0f;
    readonly float explosionRadius = 2.0f;

    public float attackDamage = 50.0f;

    Vector3 originPosition;

    void FixedUpdate()
    {
        // deactivate projectile if it exceeds the maximum travel distance
        if ((originPosition - transform.position).sqrMagnitude > maxTravelDistance * maxTravelDistance)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // handle collision with enemy objects
        if (other.CompareTag("Enemy"))
        {
            // log damage and projectile scale for debugging
            Debug.Log("Hit enemy! Scale: " + transform.localScale.x + " Damage: " + transform.localScale.x * attackDamage);

            // apply damage to the enemy based on scale of the projectile
            if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(attackDamage * transform.localScale.x);
            }
        }

        // Check for other enemies within the explosion radius and apply damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != other.gameObject)
            {
                if (collider.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage(attackDamage * transform.localScale.x);
                }
            }
        }

        // deactivate projectile after triggering
        Deactivate();
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // used to be able to correctly set origin of projectile with each use
    public void SetOrigin(Vector3 origin)
    {
        originPosition = origin;
    }
}
