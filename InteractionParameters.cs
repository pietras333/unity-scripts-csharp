using UnityEngine;

/// <summary>
/// Manages parameters related to interaction distances and settings.
/// </summary>
public class InteractionParameters : MonoBehaviour
{
    #region Serialized Fields

    [Header("Interaction Parameters")]
    [Space]
    [Header("Configuration")]
    [Header("Touch To Touch")]
    [SerializeField]
    private float interactionRange = 2f; // Range within which interaction is possible
    #endregion

    #region Public Methods

    /// <summary>
    /// Retrieves the touch-to-touch interaction parameters.
    /// </summary>
    /// <returns>TouchToTouchParameters object with interaction range.</returns>
    public TouchToTouchParameters GetTouchToTouchParameters()
    {
        return new TouchToTouchParameters(interactionRange);
    }

    #endregion
}

/// <summary>
/// Holds parameters for touch-to-touch interactions, such as interaction range.
/// </summary>
public class TouchToTouchParameters
{
    #region Fields

    public float interactableRange; // Range within which interaction is possible
    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of TouchToTouchParameters with the specified range.
    /// </summary>
    /// <param name="interactableRange">The range within which interaction is possible.</param>
    public TouchToTouchParameters(float interactableRange)
    {
        this.interactableRange = interactableRange;
    }

    #endregion
}
