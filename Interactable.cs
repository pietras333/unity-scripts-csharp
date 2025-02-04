using UnityEngine;

/// <summary>
/// Represents an object that can be interacted with.
/// </summary>
public class Interactable : MonoBehaviour
{
    #region Serialized Fields

    [Header("Interactable")]
    [Space]
    [Header("References")]
    [SerializeField]
    public Camera cam;

    [SerializeField]
    public InteractionExecution interactionExecution; // Reference to interaction execution script

    [SerializeField]
    public ViewController viewController;

    [SerializeField]
    public Locomotion locomotion;

    [SerializeField]
    public InputReceiver inputReceiver;

    [SerializeField]
    public LoadoutManager loadoutManager;

    [SerializeField]
    public GameObject playerModel;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initiates interaction with the object.
    /// This method calls the ExecuteServerRpc method on the InteractionExecution component.
    /// </summary>
    public void Interact()
    {
        if (interactionExecution != null)
        {
            interactionExecution.ExecuteServerRpc(); // Execute interaction on the server
        }
        else
        {
            Debug.LogWarning("InteractionExecution component is not assigned.");
        }
    }

    #endregion
}

/// <summary>
/// Configuration class for interactable objects, holding their name and interactability status.
/// </summary>
public class GetInteractableConfiguration
{
    #region Fields

    public string interactableName; // Name of the interactable
    public bool interactable; // Flag indicating if the object is interactable
    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the GetInteractableConfiguration class.
    /// </summary>
    /// <param name="interactableName">Name of the interactable.</param>
    /// <param name="interactable">Flag indicating if the object is interactable.</param>
    public GetInteractableConfiguration(string interactableName, bool interactable)
    {
        this.interactableName = interactableName;
        this.interactable = interactable;
    }

    #endregion
}
