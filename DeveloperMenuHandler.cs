using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles network operations for starting client, host, and server from a developer menu.
/// </summary>
public class DeveloperMenuHandler : MonoBehaviour
{
    #region Fields

    [Header("Developer Menu Handler")]
    [Space]
    [HideInInspector]
    private NetworkManager networkManager; // Reference to the NetworkManager

    [SerializeField]
    private Camera sceneCamera;
    #endregion Fields

    #region Unity Methods

    private void Start()
    {
        // Find the NetworkManager in the scene
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found in the scene!");
        }
    }

    #endregion Unity Methods

    #region Public Methods

    /// <summary>
    /// Starts the network as a client.
    /// </summary>
    public void NetworkStartClient()
    {
        if (networkManager != null)
        {
            networkManager.StartClient();
            sceneCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("NetworkManager is null. Cannot start client.");
        }
    }

    /// <summary>
    /// Starts the network as a host.
    /// </summary>
    public void NetworkStartHost()
    {
        if (networkManager != null)
        {
            networkManager.StartHost();
            sceneCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("NetworkManager is null. Cannot start host.");
        }
    }

    /// <summary>
    /// Starts the network as a server.
    /// </summary>
    public void NetworkStartServer()
    {
        if (networkManager != null)
        {
            networkManager.StartServer();
            sceneCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("NetworkManager is null. Cannot start server.");
        }
    }

    #endregion Public Methods
}
