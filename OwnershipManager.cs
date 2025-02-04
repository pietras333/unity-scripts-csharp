using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages ownership transfer for networked objects.
/// Allows clients to request ownership, which is handled by the server.
/// </summary>
public class OwnershipManager : NetworkBehaviour
{
    #region Fields

    [HideInInspector]
    private NetworkObject networkObject; // Reference to the NetworkObject component
    #endregion Fields

    #region Unity Methods

    private void Start()
    {
        // Ensure the NetworkObject component is attached and get its reference
        networkObject = GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("NetworkObject component is missing on this GameObject.");
        }
    }

    #endregion Unity Methods

    #region Public Methods

    /// <summary>
    /// Requests ownership transfer to the client.
    /// Clients send the request to the server, and the server handles the transfer.
    /// </summary>
    public void RequestOwnership()
    {
        if (IsServer)
        {
            // Server can directly change ownership
            TransferOwnershipToLocalClient();
        }
        else
        {
            // Clients need to request ownership through the server
            RequestOwnershipServerRpc();
        }
    }

    #endregion Public Methods

    #region Server Methods

    /// <summary>
    /// Server RPC to handle ownership transfer requests from clients.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ServerRpcParams rpcParams = default)
    {
        // Change ownership to the client that sent the request
        TransferOwnership(rpcParams.Receive.SenderClientId);
    }

    #endregion Server Methods

    #region Private Methods

    /// <summary>
    /// Transfers ownership to the local client.
    /// This method is called when the server is processing the request.
    /// </summary>
    private void TransferOwnershipToLocalClient()
    {
        if (networkObject != null)
        {
            networkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.LogError("NetworkObject is not assigned.");
        }
    }

    /// <summary>
    /// Transfers ownership to a specified client.
    /// </summary>
    /// <param name="clientId">The ID of the client to whom ownership will be transferred.</param>
    private void TransferOwnership(ulong clientId)
    {
        if (networkObject != null)
        {
            networkObject.ChangeOwnership(clientId);
        }
        else
        {
            Debug.LogError("NetworkObject is not assigned.");
        }
    }

    #endregion Private Methods
}
