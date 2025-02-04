using UnityEngine;

/// <summary>
/// Represents a base class for interaction execution statements, handling availability and requirement checks.
/// </summary>
public class InteractionExecutionStatement : MonoBehaviour
{
    #region Serialized Fields

    [Header("Interaction Execution Statement")]
    [Space]
    [Header("Configuration")]
    [SerializeField]
    private bool isAvailable = true; // Flag indicating if the interaction is available
    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the interaction statement if available.
    /// </summary>
    public virtual void SetStatement()
    {
        // Exit if interaction is not available
        if (!isAvailable)
        {
            return;
        }

        // Add specific logic here to set the interaction statement
        // For example: initializing or configuring statement-specific parameters
    }

    /// <summary>
    /// Checks if all requirements for the interaction statement are met.
    /// </summary>
    /// <returns>True if requirements are met, false otherwise.</returns>
    public virtual bool MetRequirements()
    {
        // Return false if interaction is not available
        if (!isAvailable)
        {
            return false;
        }

        // Add specific logic here to check if requirements are met
        // For example: validating conditions or dependencies
        return true;
    }

    #endregion
}
