using UnityEngine;

public class InBoxCheck : MonoBehaviour
{
    #region Serialized Fields

    [Header("In Box Check")]
    [Space]
    [Header("References")]
    [SerializeField]
    private Pickable pickableBase; // Reference to the base Pickable object to compare against

    [HideInInspector]
    public bool isInBox = false; // Flag indicating if a compatible Pickable object is in the box
    #endregion

    #region Trigger Handling

    /// <summary>
    /// Called when another collider enters the trigger.
    /// Checks if the collider contains a Pickable component and if its item matches the base Pickable item.
    /// </summary>
    /// <param name="collider">The collider that triggered the event.</param>
    private void OnTriggerEnter(Collider collider)
    {
        // Attempt to get the Pickable component from the collider's GameObject
        Pickable pickableTarget = collider.GetComponent<Pickable>();

        // If no Pickable component is found, return
        if (pickableTarget == null)
        {
            return;
        }

        // Compare the item names of the base and target Pickable objects
        if (
            pickableTarget.GetItemConfiguration().itemName
            == pickableBase.GetItemConfiguration().itemName
        )
        {
            isInBox = true; // Set isInBox to true if item names match
        }
        else
        {
            isInBox = false; // Otherwise, set isInBox to false
        }
    }

    #endregion
}
