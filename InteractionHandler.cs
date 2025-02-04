using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages interactions between the player and interactable objects.
/// </summary>
public class InteractionHandler : NetworkBehaviour
{
    #region Serialized Fields

    [Header("Interaction Handler")]
    [Space]
    [Header("References")]
    [Header("Scripts")]
    [SerializeField]
    private LoadoutManager loadoutManager;

    [SerializeField]
    private InteractionParameters interactionParameters; // Scriptable object holding interaction parameters

    [SerializeField]
    private InputReceiver inputReceiver; // Receives player inputs

    // [SerializeField]
    // private WeaponManager weaponManager; // Manages weapons

    [SerializeField]
    private PickupHandler pickupHandler; // Handles picking up objects

    [SerializeField]
    private ViewController viewController;

    [SerializeField]
    private Locomotion locomotion;

    [Header("Objects")]
    [SerializeField]
    GameObject playerModel;

    [SerializeField]
    Camera cam;

    [SerializeField]
    private Transform camTransform; // Main camera for raycasting

    [HideInInspector]
    private RaycastHit interactionHit; // Information about the raycast hit

    [HideInInspector]
    private bool canMove = true;

    [HideInInspector]
    private Interactable interactable;
    #endregion

    #region Unity Callbacks

    // Update is called once per frame
    private void Update()
    {
        HandleInteraction(); // Handle interactions every frame
        locomotion.canMove = canMove;
        if (!canMove)
        {
            locomotion.playerRigidbody.velocity = Vector3.zero;
        }
        ExitOnEscape();
    }

    #endregion

    #region Interaction Handling

    /// <summary>
    /// Handles interactions by performing a raycast to detect interactable objects and process interactions.
    /// </summary>
    private void HandleInteraction()
    {
        // Perform a raycast to detect interactable objects within range
        if (
            Physics.Raycast(
                camTransform.position,
                camTransform.forward,
                out interactionHit,
                interactionParameters.GetTouchToTouchParameters().interactableRange
            )
        )
        {
            interactable = interactionHit.transform.GetComponent<Interactable>();

            if (interactable == null)
            {
                return; // Exit if no Interactable component found
            }
            Debug.Log(interactable.name);

            // Check for interaction input and perform interaction
            if (Input.GetKeyDown(inputReceiver.GetInteractionInput().useKey))
            {
                if (
                    interactable.TryGetComponent<InteractionExecution>(
                        out InteractionExecution interactionExecution
                    )
                )
                {
                    interactable.interactionExecution = interactionExecution;
                }

                if (
                    interactable.GetComponent<NetworkObject>().IsOwner
                    && interactionExecution.isOccupied.Value
                )
                {
                    if (interactionExecution.stopInteractionOnExecution)
                    {
                        StopInteracting(interactionExecution);
                        return;
                    }
                    return;
                }
                else if (
                    !interactable.GetComponent<NetworkObject>().IsOwner
                    && interactionExecution.isOccupied.Value
                )
                {
                    return;
                }
                interactable.GetComponent<OwnershipManager>().RequestOwnership();
                canMove = interactionExecution.canMoveWhileInteracting;
                if (interactionExecution.onlyOneUser)
                {
                    interactionExecution.SetIsOccupiedServerRpc(true);
                }
                interactionExecution.interactable = interactable;
                interactionExecution.ExecuteServerRpc();
                interactable.viewController = viewController;
                interactable.locomotion = locomotion;
                interactable.cam = cam;
                interactable.inputReceiver = inputReceiver;
                interactable.playerModel = playerModel;
                interactable.loadoutManager = loadoutManager; // Perform the interaction
                interactionExecution.OnInteractionEnter();
            }
        }
    }

    private void StopInteracting(InteractionExecution interactionExecution)
    {
        interactionExecution.SetIsOccupiedServerRpc(false);
        interactable.viewController.canRotate = true;
        interactionExecution.interactable = null;
        interactable.viewController = null;
        interactable.locomotion = null;
        interactable.cam = null;
        interactable.inputReceiver = null;
        interactable = null;
        canMove = true;
        interactionExecution.OnInteractionExit();
    }

    private void ExitOnEscape()
    {
        if (Input.GetKey(inputReceiver.GetInteractionInput().escKey) && interactable)
        {
            if (
                interactable.TryGetComponent<InteractionExecution>(
                    out InteractionExecution interactionExecution
                )
            )
            {
                if (
                    interactable.GetComponent<NetworkObject>().IsOwner
                    && interactionExecution.isOccupied.Value
                    && interactionExecution.exitOnEsc
                )
                {
                    StopInteracting(interactionExecution);

                    return;
                }
                return;
            }
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Notifies the held weapon about the interaction state.
    /// </summary>
    /// <param name="isInteracting">True if the weapon should be in interacting state, false otherwise.</param>
    private void SetWeaponInteracting(bool isInteracting)
    {
        // Weapon heldWeapon = pickupHandler.heldObject?.GetComponent<Weapon>();
        // if (heldWeapon != null)
        // {
        //     heldWeapon.isInteracting = isInteracting; // Update weapon's interaction state
        // }
    }

    #endregion
}
