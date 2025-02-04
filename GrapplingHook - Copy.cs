using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrapplingHook : Gear
{
    #region Inspector Fields

    [Header("Grappling Hook")]
    [Header("References")]
    [Tooltip("Prefab for grappling hook particles.")]
    [SerializeField]
    private GameObject grapplingParticlePrefab;

    [Tooltip("Prefab for the line piece of the grappling hook.")]
    [SerializeField]
    private GameObject linePiecePrefab;

    [Tooltip("Prefab for the end piece of the grappling hook line.")]
    [SerializeField]
    private GameObject linePieceEndPrefab;

    [Tooltip("Prefab for the attachment of the grappling hook line.")]
    [SerializeField]
    private GameObject lineAttachmentPrefab;

    [Header("Configuration")]
    [Tooltip("Maximum distance to shoot the grappling hook.")]
    [SerializeField]
    private float shootDistance = 10f;

    [Tooltip("Layer mask to identify hookable surfaces.")]
    [SerializeField]
    private LayerMask hookableLayer;

    #endregion

    #region Private Fields

    [HideInInspector]
    private RaycastHit hookHit;

    [HideInInspector]
    private RaycastHit groundHit;

    #endregion

    public override void ExecuteChargeUtility()
    {
        ShootGrapplingHook();
    }

    // Initiates the grappling hook shooting process
    private void ShootGrapplingHook()
    {
        Debug.Log("Attempting to shoot grappling hook");

        if (TryGetHookHit())
        {
            Debug.Log("Hook hit detected");

            if (TryGetGroundHit())
            {
                Debug.Log("Ground hit detected");

                if (IsWithinShootDistance())
                {
                    Debug.Log("Within shoot distance");

                    if (IsServer)
                    {
                        Debug.Log("Server: Creating grappling line");
                        CreateGrapplingLine(hookHit.point, groundHit.point, hookHit.normal);
                    }
                    else
                    {
                        Debug.Log("Client: Requesting server to create grappling line");
                        CreateGrapplingLineServerRpc(
                            hookHit.point,
                            groundHit.point,
                            hookHit.normal
                        );
                    }
                }
            }
        }
    }

    // Performs a raycast to detect if the hook hits a valid surface
    private bool TryGetHookHit()
    {
        return Physics.Raycast(
            cam.transform.position,
            cam.transform.forward,
            out hookHit,
            shootDistance,
            hookableLayer
        );
    }

    // Performs a raycast to detect the ground below the hook hit point
    private bool TryGetGroundHit()
    {
        return Physics.Raycast(
            hookHit.point - cam.forward * 0.01f,
            Vector3.down,
            out groundHit,
            shootDistance,
            hookableLayer
        );
    }

    // Checks if the ground hit is within the shoot distance
    private bool IsWithinShootDistance()
    {
        return Mathf.Abs(hookHit.point.y - groundHit.point.y) <= shootDistance;
    }

    // Server RPC to create the grappling line
    [ServerRpc]
    private void CreateGrapplingLineServerRpc(
        Vector3 hookHitPoint,
        Vector3 groundHitPoint,
        Vector3 hookNormal
    )
    {
        Debug.Log("ServerRPC received: Creating grappling line");
        CreateGrapplingLine(hookHitPoint, groundHitPoint, hookNormal);
    }

    // Creates the grappling line from hook hit point to ground hit point
    private void CreateGrapplingLine(
        Vector3 hookHitPoint,
        Vector3 groundHitPoint,
        Vector3 hookNormal
    )
    {
        Debug.Log("Creating grappling line");

        float numberOfLinePieces = CalculateNumberOfLinePieces(hookHitPoint, groundHitPoint);

        Quaternion attachmentRotation = Quaternion.LookRotation(
            Vector3.Cross(hookNormal, Vector3.up),
            hookNormal
        );

        GameObject lineAttachment = Instantiate(
            lineAttachmentPrefab,
            hookHitPoint + hookNormal * 0.5f,
            attachmentRotation
        );
        NetworkObject networkObject = lineAttachment.GetComponent<NetworkObject>();
        networkObject.Spawn();

        Vector3 nextLinePosition =
            hookHitPoint
            + hookNormal * lineAttachmentPrefab.transform.localScale.x
            + Vector3.up * 0.25f;

        List<ulong> linePieceNetworkObjectIds = new List<ulong>();
        List<GameObject> linePieces = new List<GameObject>();

        for (int i = 0; i < numberOfLinePieces - 1; i++)
        {
            nextLinePosition = CalculateNextLinePosition(nextLinePosition);
            GameObject linePiece = Instantiate(
                ShouldUseEndPrefab(i, numberOfLinePieces) ? linePieceEndPrefab : linePiecePrefab,
                nextLinePosition,
                Quaternion.identity
            );

            NetworkObject linePieceNetworkObject = linePiece.GetComponent<NetworkObject>();
            linePieceNetworkObject.Spawn();
            linePieceNetworkObjectIds.Add(linePieceNetworkObject.NetworkObjectId);

            LineScaleIn lineScaleIn = linePiece.GetComponent<LineScaleIn>();
            lineScaleIn.smoothness += numberOfLinePieces - i;
            lineScaleIn.StartScalingIn();

            linePieces.Add(linePiece);
            ConfigureLinePiece(linePiece, linePieces, i);

            Debug.Log($"Line piece {i} created");
        }

        // Notify clients to add LineSegment components
        AddLineSegmentComponentsClientRpc(linePieceNetworkObjectIds.ToArray());
    }

    // Client RPC to add LineSegment components to the line pieces
    [ClientRpc]
    private void AddLineSegmentComponentsClientRpc(ulong[] linePieceNetworkObjectIds)
    {
        List<GameObject> linePieces = new List<GameObject>();

        foreach (var networkObjectId in linePieceNetworkObjectIds)
        {
            if (
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
                    networkObjectId,
                    out var networkObject
                )
            )
            {
                linePieces.Add(networkObject.gameObject);
            }
        }

        foreach (GameObject linePiece in linePieces)
        {
            LineSegment lineSegment = linePiece.AddComponent<LineSegment>();
            lineSegment.LineSegmentIndex = linePieces.IndexOf(linePiece);
            lineSegment.ThisLineSegments = linePieces;
        }
    }

    // Calculates the number of line pieces required
    private int CalculateNumberOfLinePieces(Vector3 hookHitPoint, Vector3 groundHitPoint)
    {
        return Mathf.Max(
            1,
            (int)((hookHitPoint.y - groundHitPoint.y) / linePiecePrefab.transform.localScale.y)
        );
    }

    // Calculates the position for the next line piece
    private Vector3 CalculateNextLinePosition(Vector3 currentPosition)
    {
        return new Vector3(
            currentPosition.x,
            currentPosition.y - linePiecePrefab.transform.localScale.y,
            currentPosition.z
        );
    }

    // Determines if the end piece prefab should be used for the line piece
    private bool ShouldUseEndPrefab(int index, float totalPieces)
    {
        return index > (int)(totalPieces - totalPieces / 3) || index < (int)(totalPieces / 3);
    }

    // Configures the properties of each line piece
    private void ConfigureLinePiece(GameObject linePiece, List<GameObject> linePieces, int index)
    {
        if (index == 0)
        {
            Destroy(linePiece.GetComponent<HingeJoint>());
            linePiece.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            linePiece.GetComponent<HingeJoint>().connectedBody = linePieces[index - 1]
                .GetComponent<Rigidbody>();
        }
    }
}
