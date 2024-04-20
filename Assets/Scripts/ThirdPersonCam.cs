using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerObj;
    [SerializeField] Transform combatLookAt;

    void Update()
    {
        // calculate directional vector to the combat target on the horizontal plane
        Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);

        // normalize the vector to ensure consistency
        orientation.forward = dirToCombatLookAt.normalized;

        // align orientation and player's forward direction with the combat target
        playerObj.forward = dirToCombatLookAt.normalized; 
    }
}
