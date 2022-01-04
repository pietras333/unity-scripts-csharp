using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunV2: MonoBehaviour
{
    [Header("Wall Run")]
    [Header("CORE")]
    [SerializeField] PlayerMovement playerMovementRef;
    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] CameraMovement cameraMovementRef;
    [Header("Customization")]
    [SerializeField] float wallDetectionDistance = 1;
    [SerializeField] float groundDetecionDistance = 1;
    [SerializeField] float wallRunGravity = 150;
    [SerializeField] public float cameraTilt = 25f;
    [SerializeField] public float cameraSmoothness = 5;
    private void FixedUpdate()
    {
        if ((isWallRunningLeft() || isWallRunningRight()) && ItsEnoughHigh() && !playerMovementRef.isGrounded) StartWallRunning();
        if (!isWallRunningLeft() || !isWallRunningRight() || !ItsEnoughHigh()) StopWallRunning();
    }
    private void Update()
    {
        ApplyCameraTilt();
    }
    private void Start()
    {
        playerRigidbody = this.GetComponent<Rigidbody>();
    }
    public bool isWallRunningRight()
    {
        if (Physics.Raycast(orientation.position, orientation.right, wallDetectionDistance))
            return true;
        else return false;
    }
    public bool isWallRunningLeft()
    {
        if (Physics.Raycast(orientation.position, -orientation.right, wallDetectionDistance))
            return true;
        else return false;
    }
    public bool ItsEnoughHigh()
    {
        RaycastHit hit;
        Physics.Raycast(orientation.position, -orientation.up, out hit, groundDetecionDistance);
        if (hit.collider == null) return true;
        else return false;
    }
    public Vector3 DetectSide()
    {
        if (playerMovementRef.inputX == 1) return Vector3.right;
        else if (playerMovementRef.inputX == -1) return -Vector3.right;
        else return Vector3.zero;
    }
    public float CameraTilt()
    {
        if (isWallRunningLeft() && !playerMovementRef.isGrounded)
            return -cameraTilt;
        else if (isWallRunningRight() && !playerMovementRef.isGrounded)
            return cameraTilt;
        else return 0;
    }
    public void StartWallRunning()
    {
        playerRigidbody.useGravity = false;
        playerRigidbody.AddForce(-orientation.up * wallRunGravity * Time.fixedDeltaTime);
        cameraMovementRef.cameraTilt = Mathf.Lerp(cameraMovementRef.cameraTilt, CameraTilt(), cameraSmoothness * Time.fixedDeltaTime);
        if (Input.GetKeyDown(playerMovementRef.jumpKey)) ApplySideForce(500);
    }
    public void StopWallRunning()
    {
        playerRigidbody.useGravity = true;
    }
    public void ApplySideForce(float force)
    {
        playerRigidbody.AddForce(GetSideVector() * GetSideJumpForce() * Time.fixedDeltaTime, ForceMode.Impulse);
    }
    public void ApplyCameraTilt()
    {
        cameraMovementRef.cameraTilt = Mathf.Lerp(cameraMovementRef.cameraTilt, CameraTilt(), cameraSmoothness * Time.deltaTime);
    }
    public float GetSideJumpForce()
    {
        return (playerMovementRef.jumpForce / 4.5f) + playerMovementRef.actualSpeed * Time.fixedDeltaTime;
    }
    public Vector3 GetSideVector()
    {
        if (isWallRunningLeft()) return new Vector3(1, 1, 0);
        else if (isWallRunningRight()) return new Vector3(-1, 1, 0);
        else return new Vector3(0, 0, 0);
    }
}
