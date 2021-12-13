using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMechanic : MonoBehaviour
{
    /// Add to whatever
    [Header("Rope Mechanic")]
    [Header("Core")]
    [SerializeField] GameObject player;
    [SerializeField] Transform mainCamera;
    [SerializeField] RaycastHit hit;
    [SerializeField] Rigidbody playerRigidbody;
    [HideInInspector] SpringJoint ropeJoint;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] bool shootRope;
    [SerializeField] bool pullRope;
    [SerializeField] int eCount = 0;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [HideInInspector] Vector3 animatedEndPos;
    [SerializeField] float distToAnch;
    [Header("Customization")]
    [SerializeField] KeyCode ropeKey;
    [SerializeField] float maxRopeRange;
    public void Update()
    {
        if (Input.GetKeyDown(ropeKey))
            eCount += 1;
        if (eCount == 1 && Input.GetKeyDown(ropeKey))
            ShootRope();
        if (eCount == 2 && shootRope)
            PullPlayerTowardsAnchor(endPos, distToAnch);
        if (eCount == 3)
            EndRope();
        if (shootRope && ropeJoint != null)
            DrawRopeAndConfigureJoint(endPos, distToAnch);
    }
    public void ShootRope()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, maxRopeRange))
        {
            endPos = hit.point;
            distToAnch = hit.distance;
            shootRope = true;
            ropeJoint = player.AddComponent<SpringJoint>();
            ropeJoint.autoConfigureConnectedAnchor = false;
            lineRenderer.SetWidth(0.1f, 0.0f);
        }
    }
    public void DrawRopeAndConfigureJoint(Vector3 endPosition, float distanceToAnchor)
    {
        animatedEndPos = Vector3.Lerp(animatedEndPos, endPosition, 5f * Time.deltaTime);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, animatedEndPos);
        ropeJoint.connectedAnchor = endPos;
        ropeJoint.minDistance = 1f;
        ropeJoint.maxDistance = distanceToAnchor + 3.5f;
    }
    public void PullPlayerTowardsAnchor(Vector3 anchorPosition, float distanceToAnchor)
    {
        if (distanceToAnchor > 3f)
            playerRigidbody.AddForce((anchorPosition - transform.position).normalized * (distanceToAnchor * .25f), ForceMode.VelocityChange);
    }
    public void EndRope()
    {
        Destroy(ropeJoint, 0f);
        lineRenderer.SetWidth(0f, 0f);
        shootRope = false;
        eCount = 0;
        playerRigidbody.AddForce(transform.up * 3f, ForceMode.Impulse);
    }
}
