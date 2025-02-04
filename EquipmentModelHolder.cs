using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentModelHolder : NetworkBehaviour
{
    #region Serialization
    [Header("Equipment Model Holder")]
    [Space]
    [Header("References")]
    [Header("Body parts")]
    [SerializeField]
    private GameObject[] head;

    [SerializeField]
    private GameObject[] eyes;

    [SerializeField]
    private GameObject[] ears;

    [SerializeField]
    private GameObject[] shoulders;

    [SerializeField]
    private GameObject[] hands;

    [SerializeField]
    private GameObject[] chest;

    [SerializeField]
    private GameObject[] back;

    [SerializeField]
    private GameObject[] hips;

    [SerializeField]
    private GameObject[] legs;

    [SerializeField]
    private GameObject[] knees;

    [SerializeField]
    private GameObject[] feet;
    #endregion

    #region Initialization
    /// <summary>
    /// Called when the script is initialized.
    /// </summary>
    public void Awake()
    {
        // Create a dictionary to store body parts for easy access
        bodyPartsDictionary = new Dictionary<EquipmentItem.BodyPart, GameObject[]>
        {
            { EquipmentItem.BodyPart.Head, head },
            { EquipmentItem.BodyPart.Eyes, eyes },
            { EquipmentItem.BodyPart.Ears, ears },
            { EquipmentItem.BodyPart.Shoulders, shoulders },
            { EquipmentItem.BodyPart.Hands, hands },
            { EquipmentItem.BodyPart.Chest, chest },
            { EquipmentItem.BodyPart.Back, back },
            { EquipmentItem.BodyPart.Hips, hips },
            { EquipmentItem.BodyPart.Legs, legs },
            { EquipmentItem.BodyPart.Knees, knees },
            { EquipmentItem.BodyPart.Feet, feet }
        };
    }
    #endregion

    #region Body Part Management
    private Dictionary<EquipmentItem.BodyPart, GameObject[]> bodyPartsDictionary;

    /// <summary>
    /// Returns an array of body parts for the specified body part type.
    /// </summary>
    /// <param name="part">The body part type.</param>
    /// <returns>An array of body parts.</returns>
    public GameObject[] GetBodyPartArray(EquipmentItem.BodyPart part)
    {
        // Try to get the body part array from the dictionary
        if (bodyPartsDictionary.TryGetValue(part, out GameObject[] bodyPartArray))
        {
            return bodyPartArray;
        }
        else
        {
            // Log an error if the body part array is not found
            Debug.LogError($"No array found for BodyPart: {part}");
            return null;
        }
    }
    #endregion

    #region Equipment Management
    /// <summary>
    /// Changes the equipment for the specified item and body part.
    /// </summary>
    /// <param name="equipmentItem">The equipment item.</param>
    /// <param name="isPreviewModel">Whether this is a preview model.</param>
    public void ChangeEquipment(EquipmentItem equipmentItem, bool isPreviewModel = false)
    {
        // Check if equipment data is null
        if (equipmentItem == null)
        {
            Debug.LogError("EquipmentData is null");
            return;
        }

        int id = equipmentItem.itemIndex;

        EquipmentItem.BodyPart bodyPart = equipmentItem.bodyPart;

        if (isPreviewModel)
        {
            // Set active model for preview
            SetActiveModel(id, bodyPart);
            return;
        }

        if (IsOwner)
        {
            // Request change equipment on server
            RequestChangeEquipmentServerRpc(id, bodyPart);
        }
    }
    #endregion

    #region Network RPCs
    [ServerRpc]
    /// <summary>
    /// Requests a change in equipment on the server.
    /// </summary>
    /// <param name="id">The equipment ID.</param>
    /// <param name="bodyPart">The body part type.</param>
    private void RequestChangeEquipmentServerRpc(int id, EquipmentItem.BodyPart bodyPart)
    {
        // Call client RPC to change equipment
        ChangeEquipmentClientRpc(id, bodyPart);
    }

    [ClientRpc]
    /// <summary>
    /// Changes the equipment on the client.
    /// </summary>
    /// <param name="id">The equipment ID.</param>
    /// <param name="bodyPart">The body part type.</param>
    private void ChangeEquipmentClientRpc(int id, EquipmentItem.BodyPart bodyPart)
    {
        // Set active model
        SetActiveModel(id, bodyPart);
    }
    #endregion

    #region Model Management
    /// <summary>
    /// Sets the active model for the specified equipment ID and body part.
    /// </summary>
    /// <param name="id">The equipment ID.</param>
    /// <param name="bodyPart">The body part type.</param>
    private void SetActiveModel(int id, EquipmentItem.BodyPart bodyPart)
    {
        // Get current body parts
        GameObject[] currentBodyParts = GetBodyPartArray(bodyPart);

        // Set active model
        currentBodyParts[id].SetActive(true);

        // Deactivate other body parts
        foreach (GameObject currentBodyPart in currentBodyParts)
        {
            if (currentBodyPart != currentBodyParts[id])
            {
                currentBodyPart.SetActive(false);
            }
        }
    }
    #endregion
}
