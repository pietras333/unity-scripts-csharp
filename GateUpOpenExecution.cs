using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the interaction for opening a gate smoothly using an animation curve.
/// </summary>
public class GateUpOpenExecution : InteractionExecution
{
    #region Serialized Fields

    [Header("Open Door Execution")]
    [Space]
    [Header("References")]
    [SerializeField]
    private Transform door; // Reference to the door's transform component

    [SerializeField]
    private Transform openedDoorPosition; // Target position where the door should move when opened

    [Header("Configuration")]
    [SerializeField]
    private float duration = 2f; // Duration of the door opening animation

    [SerializeField]
    private AnimationCurve smoothnessCurve; // Curve defining the smoothness of the door movement
    #endregion

    #region Server RPC

    /// <summary>
    /// Executes the server-side RPC to start the door opening process.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public override void ExecuteServerRpc()
    {
        if (!CheckRequirements())
        {
            return; // Exit if interaction requirements are not met
        }

        if (door == null)
        {
            Debug.LogWarning("Door transform is not assigned.");
            return; // Exit if door transform is not assigned
        }

        // Stop any existing coroutine to ensure only one door opening coroutine runs at a time
        StopCoroutine(nameof(OpenDoor));
        StartCoroutine(OpenDoor()); // Start the coroutine to open the door
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Coroutine that smoothly animates the door opening to the target position.
    /// </summary>
    private IEnumerator OpenDoor()
    {
        float timeElapsed = 0f; // Timer to track the elapsed time
        Vector3 initialPosition = door.position; // Store the initial position of the door

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime; // Increment elapsed time
            float normalizedTime = timeElapsed / duration; // Normalize time to a [0, 1] range
            float curveValue = smoothnessCurve.Evaluate(normalizedTime); // Evaluate the animation curve

            // Move the door smoothly based on the curve value
            door.position = Vector3.Lerp(initialPosition, openedDoorPosition.position, curveValue);
            yield return null; // Wait for the next frame
        }

        // Ensure the door reaches the final position
        door.position = openedDoorPosition.position;
    }

    #endregion
}
