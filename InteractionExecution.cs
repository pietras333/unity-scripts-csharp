using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages the execution of interactions in a networked environment, including requirements checking and optional camera shaking.
/// </summary>
public class InteractionExecution : NetworkBehaviour
{
    #region Serialized Fields

    [Header("Interaction Execution")]
    [Space]
    [Header("Configuration")]
    [SerializeField]
    private bool shakeOnExecution = true; // Flag to trigger camera shake upon execution

    [SerializeField]
    public NetworkVariable<bool> isOccupied = new NetworkVariable<bool>();

    [ServerRpc(RequireOwnership = false)]
    public void SetIsOccupiedServerRpc(bool occupied)
    {
        isOccupied.Value = occupied;
    }

    [SerializeField]
    public bool stopInteractionOnExecution = false;

    [SerializeField]
    public bool exitOnEsc = false;

    [SerializeField]
    public bool onlyOneUser = false;

    [SerializeField]
    public bool canMoveWhileInteracting = true;

    [HideInInspector]
    private CameraShakeHandler cameraShakeHandler; // Cached reference to the CameraShakeHandler component

    [SerializeField]
    private List<InteractionExecutionStatement> statements; // List of statements to validate before execution

    [SerializeField]
    public Interactable interactable;
    #endregion


    #region Initialization

    /// <summary>
    /// Initializes the CameraShakeHandler reference on start.
    /// </summary>
    private void Start()
    {
        // Cache the CameraShakeHandler component if it exists in the scene
        cameraShakeHandler = FindObjectOfType<CameraShakeHandler>();
    }

    public virtual void Update() { }

    public virtual void OnInteractionExit() { }

    public virtual void OnInteractionEnter() { }
    #endregion

    #region Server RPC Execution

    /// <summary>
    /// Executes the interaction on the server.
    /// This method is called over the network and checks requirements before executing the interaction.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public virtual void ExecuteServerRpc()
    {
        // Only execute if requirements are met
        if (!CheckRequirements())
        {
            return;
        }

        // Trigger camera shake if configured
        if (cameraShakeHandler != null && shakeOnExecution)
        {
            cameraShakeHandler.ShakeOnce();
        }
    }

    public virtual void UpdateCameraPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        interactable.viewController.cameraHolder.transform.SetPositionAndRotation(
            position,
            rotation
        );
    }

    #endregion

    #region Requirements Checking

    /// <summary>
    /// Checks if all requirements for the interaction are met based on the provided statements.
    /// </summary>
    /// <returns>True if all requirements are met, false otherwise.</returns>
    protected bool CheckRequirements()
    {
        // Iterate through all statements to check their requirements
        foreach (InteractionExecutionStatement statement in statements)
        {
            // Set the statement before checking requirements
            statement.SetStatement();

            // Check if the current statement's requirements are met
            if (!statement.MetRequirements())
            {
                return false; // Exit early if any requirement is not met
            }
        }

        // Trigger camera shake if configured
        if (cameraShakeHandler != null && shakeOnExecution)
        {
            cameraShakeHandler.ShakeOnce();
        }
        return true; // All requirements are satisfied
    }

    #endregion
}
