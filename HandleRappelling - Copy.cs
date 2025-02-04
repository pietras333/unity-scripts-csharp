using System;
using Unity.Netcode;
using UnityEngine;

public class HandleRappelling : NetworkBehaviour
{
    #region Inspector Fields

    [Header("Handle Rappelling")]
    [Space]
    [Header("References")]
    [Tooltip("The camera transform used to detect the rope end.")]
    [SerializeField]
    private Transform cam;

    [Tooltip("The orientation transform of the player.")]
    [SerializeField]
    private Transform orientation;

    [Tooltip("The Rigidbody component of the player.")]
    [SerializeField]
    private Rigidbody playerRb;

    [Tooltip("The InputReceiver component to handle player inputs.")]
    [SerializeField]
    private InputReceiver inputReceiver;

    [Tooltip("The CapsuleCollider component of the player.")]
    [SerializeField]
    private CapsuleCollider playerCollider;

    [Header("Configuration")]
    [Tooltip("Radius for detecting the rope.")]
    [SerializeField]
    private float ropeDetectionRadius = 1f;

    [Tooltip("Range for grabbing the rope.")]
    [SerializeField]
    private float ropeGrabRange = 4f;

    [Tooltip("Layer mask to specify which layers the rope belongs to.")]
    [SerializeField]
    private LayerMask ropeLayer;

    [Header("Ceiling Check")]
    [Tooltip("Layer mask to specify which layers are considered as ground.")]
    [SerializeField]
    private LayerMask groundLayer;

    [Tooltip("Distance to check for ceiling above.")]
    [SerializeField]
    private float ceilingCheckDistance = 1f;

    #endregion

    #region Private Fields

    private RaycastHit ropeEndHit;
    private bool isConnectedToLine;

    [Tooltip("The current end of the rope the player is connected to.")]
    [SerializeField]
    private GameObject currentLineEnd;

    private Vector3 targetPosition;

    #endregion

    private void Update()
    {
        DetectRopeEnd();
        HandleRappellingLogic();
    }

    // Detects the rope end if the player is looking at it and presses the interaction key
    private void DetectRopeEnd()
    {
        if (Physics.Raycast(cam.position, cam.forward * ropeGrabRange, out ropeEndHit, ropeLayer))
        {
            if (ropeEndHit.transform.CompareTag("RopeEnd") && Input.GetKeyDown(KeyCode.E))
            {
                ConnectToRope();
            }
        }
    }

    // Connects the player to the rope
    private void ConnectToRope()
    {
        currentLineEnd = ropeEndHit.transform.gameObject;
        Vector3 playerWorldPosition = playerRb.transform.parent.TransformPoint(
            playerRb.transform.localPosition
        );
        Vector3 direction = (playerWorldPosition - currentLineEnd.transform.position).normalized;
        targetPosition = currentLineEnd.transform.position + direction * playerCollider.radius * 2f;
        isConnectedToLine = true;
    }

    // Handles the rappelling logic including moving along the rope and disconnecting
    private void HandleRappellingLogic()
    {
        playerCollider.isTrigger = isConnectedToLine;
        playerRb.isKinematic = isConnectedToLine;

        if (isConnectedToLine)
        {
            MoveAlongRope();
            PositionPlayerOnRope();
        }

        if (Input.GetKeyUp(KeyCode.E) && isConnectedToLine)
        {
            DisconnectFromRope();
        }
    }

    // Moves the player along the rope based on input
    private void MoveAlongRope()
    {
        LineSegment currentLineSegment = currentLineEnd.GetComponent<LineSegment>();

        if (Input.GetKeyDown(KeyCode.W) && !IsCeilingAbove())
        {
            MoveUp(currentLineSegment);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown(currentLineSegment);
        }
    }

    // Checks if there is a ceiling above the player
    private bool IsCeilingAbove()
    {
        return Physics.Raycast(cam.position, Vector3.up, ceilingCheckDistance, groundLayer);
    }

    // Moves the player up along the rope
    private void MoveUp(LineSegment currentLineSegment)
    {
        int nextSegmentIndex = Mathf.Max(currentLineSegment.LineSegmentIndex - 1, 0);
        if (nextSegmentIndex <= 3)
        {
            ClimbUpFromRope();
        }
        currentLineEnd = currentLineSegment.ThisLineSegments[nextSegmentIndex].gameObject;
    }

    // Moves the player down along the rope
    private void MoveDown(LineSegment currentLineSegment)
    {
        int previousSegmentIndex = currentLineSegment.LineSegmentIndex + 1;
        if (previousSegmentIndex >= currentLineSegment.ThisLineSegments.Count)
        {
            DisconnectFromRope();
        }
        else
        {
            currentLineEnd = currentLineSegment.ThisLineSegments[previousSegmentIndex].gameObject;
        }
    }

    // Positions the player on the rope
    private void PositionPlayerOnRope()
    {
        if (currentLineEnd == null)
            return;

        Vector3 offset = new Vector3(
            currentLineEnd.transform.position.x - targetPosition.x,
            0,
            currentLineEnd.transform.position.z - targetPosition.z
        );

        Vector3 position =
            new Vector3(
                targetPosition.x,
                currentLineEnd.transform.position.y + playerCollider.height,
                targetPosition.z
            )
            + offset * 0.25f;

        UpdatePlayerPosition(position);
    }

    // Updates the player's position
    private void UpdatePlayerPosition(Vector3 position)
    {
        playerRb.transform.position = position;
    }

    // Disconnects the player from the rope
    private void DisconnectFromRope()
    {
        currentLineEnd = null;
        isConnectedToLine = false;
    }

    // Climbs up from the rope if there is a platform above
    private void ClimbUpFromRope()
    {
        RaycastHit climbHit;
        if (
            Physics.Raycast(
                orientation.position
                    + orientation.forward * playerCollider.radius
                    + Vector3.up * playerCollider.height,
                Vector3.down,
                out climbHit,
                3f,
                groundLayer
            )
        )
        {
            targetPosition = climbHit.point + Vector3.up * playerCollider.height * 2f;
            DisconnectFromRope();
        }
    }
}
