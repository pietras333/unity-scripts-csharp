using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsBehaviour : MonoBehaviour
{
    [Header("Stairs Behaviour")]
    [Header("CORE")]
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform stairsCheck;
    [SerializeField] PlayerMovement playerMovementRef;
    [Header("Customization")]
    [SerializeField] float stairsDetection = .5f;
    private void Update()
    {
        if (IsInFrontOfStairs() && playerMovementRef.moveDirection != Vector3.zero && playerMovementRef.isGrounded)
        {
            ApplyUpForce();
        }
    }
    public bool IsInFrontOfStairs()
    {
        RaycastHit hit;
        Physics.Raycast(stairsCheck.position, stairsCheck.forward, out hit, stairsDetection);
        if (hit.collider != null) return true;
        else return false;
    }
    public void ApplyUpForce()
    {
        playerRigidbody.AddForce(stairsCheck.up * 10 * Time.fixedDeltaTime, ForceMode.Impulse);

    }
}
