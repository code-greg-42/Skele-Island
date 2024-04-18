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
        if ((originPosition - transform.position).sqrMagnitude > maxTravelDistance * maxTravelDistance)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy! Scale: " + transform.localScale.x + " Damage: " + transform.localScale.x * attackDamage);
            if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(attackDamage * transform.localScale.x); // change this later based on size of projectile
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
                    enemy.TakeDamage(attackDamage * transform.localScale.x); // Apply damage to other enemies within the explosion radius
                }
            }
        }

        Deactivate();
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetOrigin(Vector3 origin)
    {
        originPosition = origin;
    }
}
