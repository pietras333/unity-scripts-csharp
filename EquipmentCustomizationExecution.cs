using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class EquipmentCustomizationExecution : InteractionExecution
{
    #region Serialized Fields

    [Header("Equipment Customization")]
    [Space]
    [SerializeField]
    Camera customizationCamera; // Camera used for equipment customization view

    [SerializeField]
    Camera mainCamera; // Main game camera

    [SerializeField]
    GameObject[] playerBodyParts; // Array of player body parts for customization

    [SerializeField]
    Transform modelSnapPosition; // Position where the model preview is instantiated

    [SerializeField]
    GameObject playerModel; // Reference to the player's model in the scene

    [SerializeField]
    EquipmentModelHolder equipmentModelHolder; // Reference to the equipment model holder that manages the equipped items

    [SerializeField]
    GameObject modelPreview; // Preview of the customized model
    #endregion

    #region Unity Methods

    /// <summary>
    /// Disables the customization camera at the start of the game.
    /// </summary>
    public void Start()
    {
        customizationCamera.enabled = false;
    }

    #endregion

    #region Server RPCs

    /// <summary>
    /// Executes the server RPC for managing equipment customization logic.
    /// </summary>
    /// <remarks>
    /// This is called to handle the equipment customization on the server side.
    /// </remarks>
    [ServerRpc(RequireOwnership = false)]
    public override void ExecuteServerRpc()
    {
        // Exit if already occupied
        if (isOccupied.Value)
        {
            return;
        }

        // Call base RPC execution
        base.ExecuteServerRpc();

        // Mark as occupied
        isOccupied.Value = true;

        // Set main camera if it hasn't been assigned
        if (!mainCamera)
        {
            mainCamera = interactable.cam;
        }

        // Enable customization camera and disable main camera
        customizationCamera.enabled = isOccupied.Value;
        mainCamera.enabled = !isOccupied.Value;
    }

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Called when the player enters the interaction area.
    /// </summary>
    public override void OnInteractionEnter()
    {
        base.OnInteractionEnter();

        // Switch to customization camera
        mainCamera = interactable.cam;
        customizationCamera.enabled = true;
        mainCamera.enabled = false;

        // Unlock cursor for equipment customization
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Get equipment model holder from player model
        equipmentModelHolder = interactable.playerModel.GetComponent<EquipmentModelHolder>();

        // Load preview of the model for customization
        LoadPreviewModel();
    }

    /// <summary>
    /// Called when the player exits the interaction area.
    /// </summary>
    public override void OnInteractionExit()
    {
        base.OnInteractionExit();

        // Restore main camera view and hide customization view
        customizationCamera.enabled = false;
        mainCamera.enabled = true;
        mainCamera = null;

        // Lock the cursor after exiting customization mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Delete the model preview
        DeletePreviewModel();
    }

    #endregion

    #region Update Loop

    /// <summary>
    /// Custom update loop for managing interactions.
    /// </summary>
    public override void Update()
    {
        // If references aren't valid, stop the update
        if (!PerformReferencesCheck())
        {
            return;
        }

        // Allow view rotation only when not occupied
        interactable.viewController.canRotate = !isOccupied.Value;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Performs a check to ensure all necessary references are valid.
    /// </summary>
    /// <returns>Returns true if all references are valid, otherwise false.</returns>
    private bool PerformReferencesCheck()
    {
        return
            !isOccupied.Value
            || !interactable
            || !interactable.viewController
            || !interactable.inputReceiver
            ? false
            : true;
    }

    /// <summary>
    /// Loads the player model preview for customization.
    /// </summary>
    public void LoadPreviewModel()
    {
        playerModel = interactable.playerModel;

        // Instantiate model preview at the snap position
        modelPreview = Instantiate(
            playerModel,
            modelSnapPosition.position,
            modelSnapPosition.rotation,
            modelSnapPosition
        );

        // Update layer of the preview model to make it render in the UI
        UpdatePreviewModelLayers();
    }

    /// <summary>
    /// Deletes the instantiated model preview.
    /// </summary>
    public void DeletePreviewModel()
    {
        Destroy(modelPreview);
    }

    /// <summary>
    /// Changes the equipment model on the player.
    /// </summary>
    /// <param name="equipmentItem">The equipment item to be applied to the model.</param>
    public void ChangeEquipment(EquipmentItem equipmentItem)
    {
        if (equipmentItem.equipmentType == EquipmentItem.EquipmentType.Weapon)
        {
            interactable.loadoutManager.SetLoadout(
                equipmentItem.weaponType,
                equipmentItem.weaponCategory,
                equipmentItem.itemIndex
            );
            return;
        }
        if (!equipmentModelHolder)
        {
            return;
        }
        // Apply equipment to the player model
        equipmentModelHolder.ChangeEquipment(equipmentItem);

        // Apply equipment to the preview model
        modelPreview.GetComponent<EquipmentModelHolder>().ChangeEquipment(equipmentItem, true);

        // Update the layers to ensure correct rendering in the UI
        UpdatePreviewModelLayers();
    }

    /// <summary>
    /// Updates the layers of the model preview so it renders correctly in the UI.
    /// </summary>
    public void UpdatePreviewModelLayers()
    {
        // Set all child body parts of the preview model to the UI layer
        foreach (Transform bodyPartTransform in modelPreview.GetComponentsInChildren<Transform>())
        {
            bodyPartTransform.transform.gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }

    #endregion
}
