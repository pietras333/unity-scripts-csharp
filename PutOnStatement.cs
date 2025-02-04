using UnityEngine;

/// <summary>
/// Handles the logic for determining if an object is placed on a specific pad by checking its state.
/// </summary>
public class PutOnStatement : InteractionExecutionStatement
{
    #region Serialized Fields

    [Header("Put On Pad Check")]
    [Space]
    [Header("References")]
    [SerializeField]
    private InBoxCheck inBoxCheck; // Reference to the InBoxCheck component for detecting if the object is in the box

    [HideInInspector]
    public bool isOnPad; // Flag indicating if the object is on the pad
    #endregion

    #region Statement Setting

    /// <summary>
    /// Sets the statement by updating the isOnPad flag based on the InBoxCheck component's status.
    /// </summary>
    public override void SetStatement()
    {
        base.SetStatement(); // Call base method to ensure any base functionality is executed
        isOnPad = inBoxCheck.isInBox; // Update isOnPad based on the current status of InBoxCheck
    }

    #endregion

    #region Requirements Checking

    /// <summary>
    /// Checks if the requirements are met by verifying if the object is on the pad.
    /// </summary>
    /// <returns>True if the object is on the pad, otherwise false.</returns>
    public override bool MetRequirements()
    {
        base.MetRequirements(); // Call base method to ensure any base requirement checks are executed
        return isOnPad; // Return whether the object is on the pad
    }

    #endregion
}
