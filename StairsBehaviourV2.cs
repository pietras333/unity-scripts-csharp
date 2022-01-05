using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsBehaviourV2 : MonoBehaviour
{
    [Header("Stairs Behaviour")]
    [Header("CORE")]
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform lowerRay;
    [SerializeField] Transform upperRay;
    [SerializeField] PlayerMovement playerMovementRef;
    [Header("Customization")]
    [SerializeField] float rayLength = .5f;
    private void Update()
    {
        if (Logic() && playerMovementRef.moveDirection != Vector3.zero && playerMovementRef.isGrounded)
        {
            ApplyUpForce();
        }
    }
    public bool Logic()
    {
        if (UpperRay() && LowerRay()) return false;
        else if (LowerRay() && !UpperRay()) return true;
        else return false;
    }
    public bool UpperRay()
    {
        if (Ray(upperRay).collider != null) return true;
        else return false;
    }
    public bool LowerRay()
    {
        if (Ray(lowerRay).collider != null) return true;
        else return false;
    }
    public RaycastHit Ray(Transform origin)
    {
        RaycastHit hit;
        Physics.Raycast(origin.position, origin.forward, out hit, rayLength);
        return hit;
    }
    public void ApplyUpForce()
    {
        playerRigidbody.AddForce(Vector3.up * 10 * Time.fixedDeltaTime, ForceMode.Impulse);
    }
}
