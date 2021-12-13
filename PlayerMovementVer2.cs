using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementVer2 : MonoBehaviour
{
    /// Add to player parent
    /// crouch scale 1;0.5;1
    /// move speed 9
    /// max move speed 10
    /// sprint speed 11
    /// max sprint speed 12
    /// crouch speed 5
    /// max crouch speed 7
    /// jump force 2.25
    /// slide scale 1;0.5;1
    /// slide force 15
    /// max slide speed 100
    /// can slide true
    /// sensitivity 5
    /// max rotation x 80
    [Header("Player Movement")]
    [Header("Movement Core")]
    [SerializeField] WallRun wallRunRef;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform playerModel;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform orientation;
    [SerializeField] CapsuleCollider playerCollider;
    [Header("Orienation Vars")]
    [SerializeField] public Vector3 moveDirection;
    [SerializeField] public float actualSpeed;
    [SerializeField] float actualMaxSpeed;
    [SerializeField] float axisX;
    [SerializeField] float axisY;
    [SerializeField] public bool isGrounded;
    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouching;
    [SerializeField] public bool isJumping;
    [Header("Movement Customization")]
    [SerializeField] Vector3 crouchScale;
    [SerializeField] LayerMask groundMask;
    [SerializeField] KeyCode sprintKey;
    [SerializeField] KeyCode crouchKey;
    [SerializeField] public KeyCode jumpKey;
    [Header("Speed Customization")]
    [SerializeField] float moveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float maxSprintSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float maxCrouchSpeed;
    [SerializeField] public float jumpForce;
    [Header("Slide")]
    [Header("Core")]
    [SerializeField] public bool isSliding;
    [Header("Slide Movement Customization")]
    [SerializeField] KeyCode slideKey;
    [SerializeField] Vector3 slideScale;
    [SerializeField] float slideForce;
    [SerializeField] float maxSlideSpeed;
    [SerializeField] bool canSlide;
    [Header("Camera")]
    [Header("Core")]
    [SerializeField] Transform cam;
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;
    [SerializeField] float rotationX;
    [SerializeField] float rotationY;
    [Header("Camera Customization")]
    [SerializeField] float sensitivity;
    [SerializeField] float maxRotationX;
    [Header("Particle System")]
    [SerializeField] ParticleSystem landingParticleSystem;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Update()
    {
        GatherUserInput();
        ApplyCameraMovement();
        AdditionalMovement();
        if (!isSliding)
            ApplyMovementToUser();
        ControlUserSpeed();
    }
    public void GatherUserInput()
    {
        axisX = Input.GetAxisRaw("Horizontal");
        axisY = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        actualSpeed = playerRigidbody.velocity.magnitude;
        rotationX -= mouseY * sensitivity;
        rotationY += mouseX * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxRotationX, maxRotationX);
        moveDirection = orientation.right * axisX + orientation.forward * axisY;
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        isSprinting = Input.GetKey(sprintKey);
        isCrouching = Input.GetKey(crouchKey);
        isJumping = Input.GetKey(jumpKey);
    }
    public void ApplyMovementToUser()
    {
        if (isGrounded || wallRunRef.isWallRunningLeft || wallRunRef.isWallRunningRight)
        {
            if (isJumping)
            {
                playerRigidbody.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
                if (isSprinting)
                    playerRigidbody.AddForce(orientation.forward * .5f, ForceMode.VelocityChange);
            }
            if (isSprinting && !isCrouching)
            {
                actualMaxSpeed = maxSprintSpeed;
                playerRigidbody.AddForce(moveDirection * sprintSpeed, ForceMode.Acceleration);
            }
            else if (isCrouching && !isSprinting)
            {
                actualMaxSpeed = maxCrouchSpeed;
                playerModel.transform.localScale = crouchScale;
                playerRigidbody.AddForce(moveDirection * crouchSpeed, ForceMode.Acceleration);
            }
            else
            {
                actualMaxSpeed = maxMoveSpeed;
                playerModel.transform.localScale = new Vector3(1f, 1f, 1f);
                playerRigidbody.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
            }
        }
        else if (!isGrounded && !wallRunRef.isWallRunningLeft && !wallRunRef.isWallRunningRight)
        {
            actualMaxSpeed = maxSprintSpeed + 1.5f;
            playerRigidbody.AddForce(moveDirection * 1.75f, ForceMode.Acceleration);
            playerRigidbody.AddForce(Vector3.down * 1.2f, ForceMode.Acceleration);
            if (isCrouching && !isSprinting)
            {
                playerModel.transform.localScale = crouchScale;
                playerRigidbody.AddForce(moveDirection * crouchSpeed, ForceMode.Acceleration);
            }
        }
    }
    public void ControlUserSpeed()
    {
        playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, actualMaxSpeed);
    }
    public void ApplyCameraMovement()
    {
        orientation.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        cam.transform.rotation = Quaternion.Euler(rotationX, rotationY, wallRunRef.cameraTargetTilt);
    }
    public void AdditionalMovement()
    {
        if (Input.GetKeyDown(slideKey) && isGrounded && canSlide)
            StartSliding();
        else if (isSliding && actualSpeed < 0.25f || Input.GetKeyUp(slideKey))
            StopSliding();
    }
    public void StartSliding()
    {
        isSliding = true;
        playerModel.transform.localScale = slideScale;
        playerCollider.height = 1f;
        actualMaxSpeed = maxSlideSpeed;
        playerRigidbody.AddForce(moveDirection * slideForce, ForceMode.VelocityChange);
        canSlide = false;
    }
    public void StopSliding()
    {
        Invoke("CanSlideTrue", 2f);
        isSliding = false;
        playerModel.transform.localScale = new Vector3(1f, 1f, 1f);
        playerCollider.height = 2f;
    }
    public void CanSlideTrue()
    {
        canSlide = true;
    }
    public void OnCollisionEnter()
    {
        landingParticleSystem.Play();
    }
}
