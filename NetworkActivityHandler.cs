using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Disables specified components and game objects for remote clients in a networked game.
/// </summary>
public class NetworkActivityHandler : NetworkBehaviour
{
    #region Fields

    [Header("Disable Remote Client")]
    [Space]
    [SerializeField]
    private List<MonoBehaviour> localOnlyScripts = new List<MonoBehaviour>(); // Scripts to disable for remote clients

    [SerializeField]
    private List<Camera> cameras = new List<Camera>(); // Cameras to disable for remote clients

    [SerializeField]
    private List<AudioListener> audioListeners = new List<AudioListener>(); // Audio listeners to disable for remote clients

    [SerializeField]
    private List<GameObject> gameObjects = new List<GameObject>(); // Game objects to disable for remote clients

    [Header("Disable Local Client")]
    [Space]
    [SerializeField]
    private List<GameObject> remoteOnlyGameObjects = new List<GameObject>();
    #endregion Fields

    #region Unity Methods

    private void Start()
    {
        // Check if this instance is owned by the local client
        if (IsOwner)
        {
            EnableComponents(); // Enable components for the local client
            DisableLocalClient();
        }
        else
        {
            DisableComponents(); // Disable components for remote clients
        }
    }

    #endregion Unity Methods

    #region Private Methods

    // Enables all specified components and game objects
    private void EnableComponents()
    {
        foreach (MonoBehaviour script in localOnlyScripts)
        {
            script.enabled = true;
        }

        foreach (Camera camera in cameras)
        {
            camera.enabled = true;
        }

        foreach (AudioListener audioListener in audioListeners)
        {
            audioListener.enabled = true;
        }

        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(true);
        }
    }

    // Disables all specified components and game objects
    private void DisableComponents()
    {
        foreach (MonoBehaviour script in localOnlyScripts)
        {
            script.enabled = false;
        }

        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
        }

        foreach (AudioListener audioListener in audioListeners)
        {
            audioListener.enabled = false;
        }

        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }

    private void DisableLocalClient()
    {
        foreach (GameObject gameObject in remoteOnlyGameObjects)
        {
            gameObject.SetActive(false);
        }
    }

    #endregion Private Methods
}
