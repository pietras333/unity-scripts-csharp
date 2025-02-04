using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComputerInteractionExecution : InteractionExecution
{
    [Header("Use Computer")]
    [Space]
    [Header("References")]
    [SerializeField]
    Transform cameraSnapPosition;

    [SerializeField]
    RectTransform cursorTransform;

    [SerializeField]
    Transform cursorHolderTransform;

    [SerializeField]
    GraphicRaycaster canvasGraphicRaycaster;

    [HideInInspector]
    private float mouseX;

    [HideInInspector]
    private float mouseY;

    [HideInInspector]
    private PointerEventData canvasPointerData;

    [Space]
    [Header("Scripts")]
    [SerializeField]
    AppModuleReader appModuleReader;

    [SerializeField]
    RawImage cursorRenderer;

    [Space]
    [Header("UI Components References")]
    [SerializeField]
    Texture2D cursorRestTexture;

    [SerializeField]
    Texture2D cursorDragTexture;

    [SerializeField]
    LayerMask uiLayer;

    [SerializeField]
    Canvas uiCanvas;

    [SerializeField]
    List<Button> uiButtons = new List<Button>();

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

    private Vector2 GetCanvasScreenPosition()
    {
        return uiCanvas.worldCamera.WorldToScreenPoint(cursorTransform.position);
    }

    private Texture2D GetCurrentCursorTexture()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            return cursorRestTexture;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            return cursorDragTexture;
        }
        return cursorRestTexture;
    }

    private List<RaycastResult> GetCanvasRaycastResults()
    {
        // Create a pointer event data object
        canvasPointerData = new PointerEventData(EventSystem.current)
        {
            position = GetCanvasScreenPosition()
        };

        // Create a list to store the results
        List<RaycastResult> pointerEventResults = new List<RaycastResult>();

        // Perform the raycast
        canvasGraphicRaycaster.Raycast(canvasPointerData, pointerEventResults);

        return pointerEventResults;
    }

    private void UpdateCursorPosition()
    {
        // Update the cursor position based on the input
        mouseX = interactable.inputReceiver.mouseX;
        mouseY = interactable.inputReceiver.mouseY;
        if (IsServer)
        {
            Vector3 newCursorPosition = (Vector3.up * mouseY + Vector3.right * mouseX) * 3;
            UpdateCursorPositionClientRpc(newCursorPosition);
        }
        else
        {
            UpdateCursorPositionServerRpc(mouseX, mouseY);
        }
    }

    public override void Update()
    {
        if (!PerformReferencesCheck())
        {
            return;
        }

        interactable.viewController.canRotate = !isOccupied.Value;

        UpdateCursorPosition();

        UpdateCameraPositionAndRotation(cameraSnapPosition.position, cameraSnapPosition.rotation);

        cursorRenderer.texture = GetCurrentCursorTexture();

        // Check if any UI elements were hit
        if (GetCanvasRaycastResults().Count > 0)
        {
            GameObject result = GetCanvasRaycastResults()[0].gameObject;

            if (result.TryGetComponent<ToolbarUI>(out ToolbarUI toolbarUI))
            {
                GameObject appUI = toolbarUI.appUI;
                ulong appID = appUI.GetComponent<NetworkObject>().NetworkObjectId;
                ulong cursorID = cursorTransform.GetComponent<NetworkObject>().NetworkObjectId;
                ulong canvasID = uiCanvas.GetComponent<NetworkObject>().NetworkObjectId;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    UpdateAppUIPosition(appID, cursorID);
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    UpdateAppParent(appID, canvasID);
                }
            }

            if (result.CompareTag("UI Icon"))
            {
                ulong id = result.GetComponent<NetworkObject>().NetworkObjectId;
                ExecuteIconAction(id);

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    result.GetComponent<RectTransform>().localScale = Vector3.one;
                }
            }
            if (result.TryGetComponent<ScrollRect>(out ScrollRect scrollRect))
            {
                Scrollbar scrollbarChild =
                    scrollRect.gameObject.GetComponentInChildren<Scrollbar>();

                float value = Input.GetAxisRaw("Mouse ScrollWheel");
                ulong id = scrollbarChild.GetComponentInParent<NetworkObject>().NetworkObjectId;
                UpdateScrollbarScrollwheel(id, value);
            }
            if (result.TryGetComponent<Scrollbar>(out Scrollbar scrollbar))
            {
                ulong id = scrollbar.GetComponentInParent<NetworkObject>().NetworkObjectId;
                float value = mouseY * 0.1f;
                UpdateScrollbar(id, value);
            }
            if (result.TryGetComponent<Button>(out Button button))
            {
                NetworkObject resultNetworkObject = result.GetComponent<NetworkObject>();

                if (!resultNetworkObject)
                    return;

                ulong resultObjectID = result.GetComponent<NetworkObject>().NetworkObjectId;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    button.GetComponent<UIAction>().ExecuteAction();
                }

                UpdateButtonEnterState(resultObjectID, canvasPointerData.position);

                UpdateButtonDownState(resultObjectID, canvasPointerData.position);

                UpdateButtonUpState(resultObjectID, canvasPointerData.position);

                if (uiButtons.Contains(button))
                {
                    return;
                }
                uiButtons.Add(button);
            }
        }

        UpdateButtonExitState();
    }

    private void UpdateAppParent(ulong appID, ulong canvasID)
    {
        if (IsServer)
        {
            GameObject app = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appID].gameObject;
            GameObject canvas = NetworkManager
                .Singleton
                .SpawnManager
                .SpawnedObjects[canvasID]
                .gameObject;

            app.GetComponent<NetworkObject>().TrySetParent(canvas.transform, true);
            app.transform.SetSiblingIndex(canvas.transform.childCount - 2);
            UpdateAppParentClientRpc(appID, canvasID);
        }
        else
        {
            UpdateAppParentServerRpc(appID, canvasID);
        }
    }

    private void UpdateAppUIPosition(ulong appID, ulong cursorID)
    {
        GameObject app = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appID].gameObject;

        if (IsServer)
        {
            app.transform.SetParent(cursorHolderTransform.transform, true);
            UpdateAppUIPositionClientRpc(appID, app.transform.position);
        }
        else
        {
            UpdateAppUIPositionServerRpc(appID, cursorID, app.transform.position);
        }
    }

    private void ExecuteIconAction(ulong id)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (IsServer)
            {
                ExecuteIconActionClientRpc(id);
            }
            else
            {
                ExecuteIconActionServerRpc(id);
            }
        }
    }

    private void UpdateScrollbarScrollwheel(ulong id, float value)
    {
        if (IsServer)
        {
            UpdateScrollbarClientRpc(id, value);
        }
        else
        {
            UpdateScrollbarServerRpc(id, value);
        }
    }

    private void UpdateScrollbar(ulong id, float value)
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsServer)
            {
                UpdateScrollbarClientRpc(id, value);
            }
            else
            {
                UpdateScrollbarServerRpc(id, value);
            }
        }
    }

    private void UpdateButtonUpState(ulong id, Vector3 pointerDataPosition)
    {
        GameObject networkObject = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[id]
            .gameObject;
        Button button = networkObject.GetComponent<Button>();
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (IsServer)
            {
                UpdateButtonUpStateClientRpc(
                    button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                    pointerDataPosition
                );
            }
            else
            {
                UpdateButtonUpStateServerRpc(
                    button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                    pointerDataPosition
                );
            }
        }
    }

    private void UpdateButtonDownState(ulong id, Vector3 pointerDataPosition)
    {
        GameObject networkObject = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[id]
            .gameObject;
        Button button = networkObject.GetComponent<Button>();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (IsServer)
            {
                UpdateButtonDownStateClientRpc(
                    button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                    pointerDataPosition
                );
            }
            else
            {
                UpdateButtonDownStateServerRpc(
                    button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                    pointerDataPosition
                );
            }
        }
    }

    private void UpdateButtonEnterState(ulong id, Vector3 pointerDataPosition)
    {
        GameObject networkObject = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[id]
            .gameObject;
        Button button = networkObject.GetComponent<Button>();
        if (IsServer)
        {
            UpdateButtonEnterStateClientRpc(
                button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                pointerDataPosition
            );
        }
        else
        {
            UpdateButtonEnterStateServerRpc(
                button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                pointerDataPosition
            );
        }
    }

    private void UpdateButtonExitState()
    {
        foreach (Button button in uiButtons)
        {
            if (IsServer)
            {
                UpdateButtonExitStateClientRpc(
                    button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                    canvasPointerData.position
                );
            }
            else
            {
                UpdateButtonExitStateServerRpc(
                    button.GetComponentInParent<NetworkObject>().NetworkObjectId,
                    canvasPointerData.position
                );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateAppParentServerRpc(ulong appID, ulong canvasID)
    {
        GameObject app = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appID].gameObject;
        GameObject canvas = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[canvasID]
            .gameObject;

        app.GetComponent<NetworkObject>().TrySetParent(canvas.transform, true);
        app.transform.SetSiblingIndex(canvas.transform.childCount - 2);
        UpdateAppParentClientRpc(appID, canvasID);
    }

    [ClientRpc]
    public void UpdateAppParentClientRpc(ulong appID, ulong canvasID)
    {
        GameObject app = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appID].gameObject;
        GameObject canvas = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[canvasID]
            .gameObject;
        app.GetComponent<NetworkObject>().TrySetParent(canvas.transform, true);
        app.transform.SetSiblingIndex(canvas.transform.childCount - 2);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateAppUIPositionServerRpc(ulong appId, ulong cursorTransform, Vector3 position)
    {
        GameObject appUI = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appId].gameObject;
        GameObject cursorHolderTransform = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[cursorTransform]
            .gameObject;
        appUI.GetComponent<NetworkObject>().TrySetParent(cursorHolderTransform, true);
        UpdateAppUIPositionClientRpc(appId, position);
    }

    [ClientRpc]
    public void UpdateAppUIPositionClientRpc(ulong appId, Vector3 position)
    {
        GameObject app = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appId].gameObject;
        app.transform.position = position;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ExecuteIconActionServerRpc(ulong appId)
    {
        ExecuteIconActionClientRpc(appId);
    }

    [ClientRpc]
    public void ExecuteIconActionClientRpc(ulong appId)
    {
        GameObject app = NetworkManager.Singleton.SpawnManager.SpawnedObjects[appId].gameObject;
        app.GetComponent<RectTransform>().localScale = Vector3.one * 0.75f;
        app.GetComponent<UIAction>().ExecuteAction();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateButtonUpStateServerRpc(ulong buttonId, Vector2 pointerPosition)
    {
        UpdateButtonUpStateClientRpc(buttonId, pointerPosition);
    }

    [ClientRpc]
    public void UpdateButtonUpStateClientRpc(ulong buttonId, Vector2 pointerPosition)
    {
        Button button = NetworkManager
            .Singleton.SpawnManager.SpawnedObjects[buttonId]
            .gameObject.GetComponentInChildren<Button>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };

        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateButtonDownStateServerRpc(ulong buttonId, Vector2 pointerPosition)
    {
        UpdateButtonDownStateClientRpc(buttonId, pointerPosition);
    }

    [ClientRpc]
    public void UpdateButtonDownStateClientRpc(ulong buttonId, Vector2 pointerPosition)
    {
        Button button = NetworkManager
            .Singleton.SpawnManager.SpawnedObjects[buttonId]
            .gameObject.GetComponentInChildren<Button>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };
        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
        button.onClick.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateButtonEnterStateServerRpc(ulong buttonId, Vector2 pointerPosition)
    {
        UpdateButtonEnterStateClientRpc(buttonId, pointerPosition);
    }

    [ClientRpc]
    public void UpdateButtonEnterStateClientRpc(ulong buttonId, Vector2 pointerPosition)
    {
        Button button = NetworkManager
            .Singleton.SpawnManager.SpawnedObjects[buttonId]
            .gameObject.GetComponentInChildren<Button>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };

        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateButtonExitStateServerRpc(ulong buttonId, Vector2 pointerPosition)
    {
        UpdateButtonExitStateClientRpc(buttonId, pointerPosition);
    }

    [ClientRpc]
    public void UpdateButtonExitStateClientRpc(ulong buttonId, Vector2 pointerPosition)
    {
        Button button = NetworkManager
            .Singleton.SpawnManager.SpawnedObjects[buttonId]
            .gameObject.GetComponentInChildren<Button>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };

        ExecuteEvents.Execute(button.gameObject, pointerData, ExecuteEvents.pointerExitHandler);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScrollbarServerRpc(ulong scrollbarId, float value)
    {
        UpdateScrollbarClientRpc(scrollbarId, value);
    }

    [ClientRpc]
    public void UpdateScrollbarClientRpc(ulong scrollbarId, float value)
    {
        Scrollbar scrollbarChild = NetworkManager
            .Singleton.SpawnManager.SpawnedObjects[scrollbarId]
            .gameObject.GetComponentInChildren<Scrollbar>();
        scrollbarChild.value += value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCursorPositionServerRpc(float mouseX, float mouseY)
    {
        Vector3 position = (Vector3.up * mouseY + Vector3.right * mouseX) * 3;
        UpdateCursorPositionClientRpc(position);
    }

    [ClientRpc]
    public void UpdateCursorPositionClientRpc(Vector3 position)
    {
        cursorTransform.transform.localPosition += position;
    }

    [ServerRpc(RequireOwnership = false)]
    public override void ExecuteServerRpc()
    {
        if (isOccupied.Value)
        {
            return;
        }

        isOccupied.Value = true;
    }

    public void ClickButtonTest()
    {
        Debug.Log("Clicked button test");
    }
}
