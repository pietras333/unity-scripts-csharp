using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [Header("Ground Detection")]

    [Header("References")]

    [Header("Objects")]
    [SerializeField] Transform groundSensor;

    [Header("Configuration")]
    [SerializeField] float detectionRange = 0.1f;
    [SerializeField] LayerMask groundLayer;

    public GroundDetectionResult CheckGrounded()
    {
        bool checkForGround = Physics.Raycast(groundSensor.position, -groundSensor.up, out RaycastHit hit, detectionRange, groundLayer);

        return new GroundDetectionResult(checkForGround, hit);
    }
}

public class GroundDetectionResult
{
    public bool checkForGround;
    public RaycastHit hitInfo;

    public GroundDetectionResult(bool checkForGround, RaycastHit hitInfo)
    {
        this.checkForGround = checkForGround;
        this.hitInfo = hitInfo;
    }
}
