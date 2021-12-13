using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    /// Add to player parent
    /// wall run gravity 2.5 with 1.0 mass scale 
    /// smoothness 2
    /// camera smoothness 10
    /// max distance to wall 1
    /// min jump height 2
    [Header("Wall Run")]
    [Header("Core")]
    [SerializeField] Transform mainCam;
    [SerializeField] Camera mainCamera;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform orientation;
    [SerializeField] PlayerMovement playerMovementRef;
    [SerializeField] public bool isWallRunningLeft;
    [SerializeField] public bool isWallRunningRight;
    [SerializeField] bool canWallRun;
    [SerializeField] RaycastHit wallRunLeftHit;
    [SerializeField] RaycastHit wallRunRightHit;
    [HideInInspector] public float cameraTargetTilt;
    [Header("Customization")]
    [SerializeField] float actualFov;
    [SerializeField] float normalFov;
    [SerializeField] float wallRunFov;
    [SerializeField] float wallRunGravity;
    [SerializeField] float smoothness;
    [SerializeField] float cameraSmoothness;
    [SerializeField] float maxDistanceToWall;
    [SerializeField] float minJumpHeight;
    public void Update()
    {
        canWallRun = Physics.Raycast(orientation.position, Vector3.down, minJumpHeight);
        actualFov = mainCamera.fieldOfView;
        if (!canWallRun)
        {
            GatherData();
            if (isWallRunningLeft || isWallRunningRight)
                StartWallRun(playerMovementRef.jumpKey, playerMovementRef.moveDirection);
            else
                StopWallRun();
        }
        else
        {
            StopWallRun();
        }
        if (isWallRunningLeft || isWallRunningRight)
            TiltCamera(15f);
        else
            cameraTargetTilt = Mathf.Lerp(cameraTargetTilt, 0f, cameraSmoothness * Time.deltaTime);
    }
    public void GatherData()
    {
        isWallRunningLeft = Physics.Raycast(orientation.position, -orientation.right, out wallRunLeftHit, maxDistanceToWall);
        isWallRunningRight = Physics.Raycast(orientation.position, orientation.right, out wallRunRightHit, maxDistanceToWall);
    }

    public void StartWallRun(KeyCode jumpKey, Vector3 moveDirection)
    {
        playerRigidbody.useGravity = false;
        playerRigidbody.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, wallRunFov, smoothness * Time.deltaTime);
        if (Input.GetKey(jumpKey))
        {
            if (moveDirection != Vector3.zero)
            {
                if (isWallRunningLeft)
                    ApplySidewayForce(orientation.right, 2f);
                else if (isWallRunningRight)
                    ApplySidewayForce(-orientation.right, 2f);
            }
        }

    }
    public void ApplySidewayForce(Vector3 origin, float force)
    {
        playerRigidbody.AddForce(origin * force / 2.5f, ForceMode.VelocityChange);
    }
    public void StopWallRun()
    {
        playerRigidbody.useGravity = true;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFov, smoothness * Time.deltaTime);
    }
    public void TiltCamera(float cameraTilt)
    {
        if (isWallRunningLeft)
            cameraTargetTilt = Mathf.Lerp(cameraTargetTilt, -cameraTilt, cameraSmoothness * Time.deltaTime);
        if (isWallRunningRight)
            cameraTargetTilt = Mathf.Lerp(cameraTargetTilt, cameraTilt, cameraSmoothness * Time.deltaTime);
    }
}
