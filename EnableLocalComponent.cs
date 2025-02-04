using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Enables or disables a specified MonoBehaviour component based on ownership in a networked game.
/// </summary>
public class EnableLocalComponent : NetworkBehaviour
{
    #region Fields

    [Header("Enable Local Component")]
    [Space]
    [SerializeField]
    private MonoBehaviour targetScript; // Reference to the MonoBehaviour component to be enabled/disabled
    #endregion Fields

    #region Unity Methods

    private void Start()
    {
        // Enable the component if this instance is owned by the local client; otherwise, disable it
        if (targetScript != null)
        {
            targetScript.enabled = IsOwner;
        }
        else
        {
            Debug.LogWarning("Target script is not assigned.");
        }
    }

    #endregion Unity Methods
}
