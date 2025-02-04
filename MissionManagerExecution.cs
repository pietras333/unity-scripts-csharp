using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MissionManagerExecution : InteractionExecution
{
    [Header("Mission Manager Execution")]
    [Header("References")]
    [SerializeField]
    private Camera missionCamera;

    [HideInInspector]
    private Camera mainCamera;

    public void Start()
    {
        missionCamera.enabled = false;
    }

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
        missionCamera.enabled = isOccupied.Value;
        mainCamera.enabled = !isOccupied.Value;
    }

    public override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        // Switch to customization camera
        mainCamera = interactable.cam;
        missionCamera.enabled = true;
        mainCamera.enabled = false;

        // Unlock cursor for equipment customization
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void OnInteractionExit()
    {
        missionCamera.enabled = false;
        mainCamera.enabled = true;
        mainCamera = null;

        // Lock the cursor after exiting customization mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
