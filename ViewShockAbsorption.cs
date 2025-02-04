using UnityEngine;

public class ViewShockAbsorption : MonoBehaviour
{
    #region Variables

    [Header("View Shock Absorption")]
    [Space]
    [Header("References")]
    [Header("Scripts")]
    [SerializeField]
    ViewParameters viewParameters; // Reference to view parameters script

    [Header("Objects")]
    [SerializeField]
    Rigidbody playerRigidbody; // Reference to the player's Rigidbody component

    [HideInInspector]
    float timeLeft; // Time left for shock absorption

    [HideInInspector]
    RaycastHit groundHit; // RaycastHit variable to detect ground

    [HideInInspector]
    float initialPosY; // Initial Y position of the camera
    #endregion

    #region Unity Callbacks

    void Start()
    {
        InitializeShockAbsorption();
    }

    void Update()
    {
        HandleShockAbsorption();
    }

    #endregion

    #region Shock Absorption Methods

    // Initialize shock absorption variables
    void InitializeShockAbsorption()
    {
        initialPosY = transform.localPosition.y; // Store initial Y position of the camera
    }

    // Handle shock absorption effect based on player's vertical velocity
    void HandleShockAbsorption()
    {
        timeLeft -= Time.deltaTime; // Decrease time left for shock absorption

        // Calculate Y position displacement based on player's vertical velocity and shock absorption parameters
        float positionDisplacementY =
            viewParameters.GetShockAbsorbtionParameters().amplitude
            * playerRigidbody.velocity.y
            * 0.25f;

        // Check if the player is falling
        if (CheckFall())
        {
            timeLeft = viewParameters.GetShockAbsorbtionParameters().duration; // Reset time left for shock absorption
        }

        // Apply shock absorption effect if time left is greater than 0
        if (timeLeft > 0)
        {
            // Smoothly move the camera's local position to absorb shock
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(0, positionDisplacementY, 0),
                viewParameters.GetShockAbsorbtionParameters().absorbtionSmothness * Time.deltaTime
            );
        }
        else
        {
            // If shock absorption time has expired, return the camera to its initial position
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(0, initialPosY, 0),
                viewParameters.GetShockAbsorbtionParameters().absorbtionSmothness * Time.deltaTime
            );
        }
    }

    // Function to check if the player is falling
    bool CheckFall()
    {
        // Check if the player is in the second falling stage (falling with no ground contact)
        bool isInSecondFallingStage =
            !Physics.Raycast(
                transform.parent.transform.position,
                Vector3.down,
                out groundHit,
                Time.deltaTime
            )
            && playerRigidbody.velocity.y < -0.3f;

        // Check if the player is in the first falling stage (falling with ground contact within a certain distance)
        bool isInFirstFallingStage = Physics.Raycast(
            transform.parent.transform.position,
            Vector3.down,
            out groundHit,
            4f
        );

        // Return true if the player is in both falling stages
        return isInFirstFallingStage && isInSecondFallingStage;
    }

    #endregion
}
