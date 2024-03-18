using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Locomotion : NetworkBehaviour
{
    [Header("Locomotion")]

    [Header("References")]

    [Header("Scripts")]
    [SerializeField] InputHandler inputHandler;
    [SerializeField] GroundDetection groundDetection;
    [SerializeField] SlopeHandler slopeHandler;

    [Header("Objects")]
    [SerializeField] public Rigidbody playerRigidbody;
    [SerializeField] Transform orientation;
    [SerializeField] CapsuleCollider playerCollider;

    [Header("Current States")]
    [SerializeField] public float currentSpeed;

    [Header("Basic Locomotion")]
    [SerializeField] bool isGrounded;
    [SerializeField] float walkSpeed = 13f;
    [SerializeField] float walkMaxSpeed = 6f;
    [SerializeField] float runSpeed = 15f;
    [SerializeField] float runMaxSpeed = 9f;
    [SerializeField] float airSpeed = 10f;
    [SerializeField] float airMaxSpeed = 15f;
    [SerializeField] float currentMaxSpeed;
    [SerializeField] float forceMultiplier = 100f;
    [SerializeField] Vector3 currentDirection;
    [HideInInspector] Vector3 lastDirection;

    [Header("Jump")]
    [SerializeField] float jumpForce = 5f;

    [Header("Slide")]
    [SerializeField] public bool isSliding;
    [SerializeField] float maxSlideTime = 2f;
    [SerializeField] float slideForce = 5;
    [SerializeField] float normalHeight = 2f;
    [SerializeField] float slideHeight = 1f;
    [SerializeField] Vector3 slideDirection;

    public void Update()
    {
        // Check if player is owner of character
        if (!IsOwner)
        {
            // If not dont do any logic
            return;
        }

        // Handle slide
        HandleSlide();

        // Update variables on runtime
        UpdateVariables();

        // Control speed by clamping its velocity
        HandleSpeedControl();

        // Set rotation to face last direction
        SetRotation();

        // Use gravity only when player is not on slope to avoid bumping
        playerRigidbody.useGravity = !slopeHandler.CheckForSlope().checkForSlope;
    }

    public void FixedUpdate()
    {
        // Check if player is owner of character
        if (!IsOwner)
        {
            // If not dont do any logic
            return;
        }

        // Handle Jumping
        HandleJump();

        // Handle basic locomotion 
        HandleMotion();
    }

    #region MAIN FUNCTIONS
    void HandleMotion()
    {
        // If player is sliding dont allow player to move
        if (isSliding)
        {
            return;
        }

        // Look for input and apply forces
        if (inputHandler.CheckForMovement())
        {
            // Move player with current force
            playerRigidbody.AddForce(currentSpeed * forceMultiplier * Time.fixedDeltaTime * currentDirection, ForceMode.Force);
        }

        // If player is in air then apply gravity force to make him speed up
        if (!isGrounded)
        {
            playerRigidbody.AddForce(9f * Time.fixedDeltaTime * -orientation.up, ForceMode.Force);
        }
    }

    void HandleJump()
    {
        // Look for input and if player is on ground in the moment he's jumping
        if (inputHandler.isJumping && isGrounded)
        {
            // Move player upward with force
            playerRigidbody.AddForce(forceMultiplier * jumpForce * orientation.up, ForceMode.Force);
        }
    }

    void UpdateVariables()
    {
        // Get speed
        currentSpeed = GetCurrentSpeed();

        // Get current max speed
        currentMaxSpeed = GetCurrentMaxSpeed();

        // Check if player is on ground
        isGrounded = groundDetection.CheckGrounded().checkForGround;

        // Get current direction
        currentDirection = GetCurrentDirection();

    }

    void HandleSpeedControl()
    {
        // If player moves faster than his max speed then reduce his velocity to max velocity obtainable with this speed
        if (playerRigidbody.velocity.magnitude > currentMaxSpeed)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * currentMaxSpeed;
        }

        // If player sents no input then isolate x and z axis and stop him on those axis
        if (!inputHandler.CheckForMovement())
        {
            Vector3 isolatedVelocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);
            playerRigidbody.velocity = Vector3.Lerp(playerRigidbody.velocity, isolatedVelocity, Time.deltaTime);
        }
    }

    void SetRotation()
    {
        // Check if player is sliding
        if (isSliding)
        {
            // If so then dont apply rotation
            return;
        }

        // Look for input
        if (inputHandler.CheckForMovement())
        {
            // Apply last known direction as last direction
            lastDirection = inputHandler.direction;
        }

        // Calculate target rotation based on last direction 
        Quaternion targetRotation = Quaternion.LookRotation(lastDirection, Vector3.up);

        // Rotate player smoothly with .Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    void HandleSlide()
    {
        // Check if player is pressing down slide key and want to move
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Assign slide direction based on player platform if its slope get slop direction and if not get base direction
            slideDirection = slopeHandler.CheckForSlope().checkForSlope ? slopeHandler.CheckForSlope().slopeMoveDirection : inputHandler.direction;

            // Then start sliding
            StartSlide();
        }

        // Check if player stopped pressing slide key and is currently sliding
        if (Input.GetKeyUp(inputHandler.slideKey) && isSliding)
        {
            // Then stop sliding
            StopSlide();
        }
    }

    void StartSlide()
    {
        // Start sliding
        isSliding = true;

        // Scale player collider down
        playerCollider.height = slideHeight;

        // Apply down force so player wont be in the air 
        playerRigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Invoke slide coroutine
        StartCoroutine(nameof(ApplySlideForce));
    }

    IEnumerator ApplySlideForce()
    {
        // Automatically refill timer to max slide time
        float time = maxSlideTime;

        // Do coroutine as long as timer expires
        while (time >= 0f)
        {
            // Check if player is on slope
            SlopeDetectionResult slopeDetectionResult = slopeHandler.CheckForSlope();

            // If player is not on slope
            if (!slopeDetectionResult.checkForSlope)
            {
                // Then decrease timer
                time -= Time.deltaTime;
            }

            // Apply force in current sliding direction 
            playerRigidbody.AddForce(slideForce * Time.fixedDeltaTime * slideDirection.normalized, ForceMode.Force);

            // Repeat cycle
            yield return null;
        }

        // After cycle stop sliding
        StopSlide();
    }

    private void StopSlide()
    {
        // Set sliding to false
        isSliding = false;

        // Scale player up to normal height
        playerCollider.height = normalHeight;

        // Stop current sliding coroutine
        StopCoroutine(nameof(ApplySlideForce));
    }

    #endregion MAIN FUNCIONS

    #region GET / SET FUNCTIONS
    float GetCurrentSpeed()
    {
        // If player is not on ground set his speed to air speed
        if (!isGrounded)
        {
            return airSpeed;
        }

        // Return runSpeed if player is running or walkSpeed if is walking
        return inputHandler.isRunning ? runSpeed : walkSpeed;
    }

    Vector3 GetCurrentDirection()
    {
        // Check for slope and return slope direction if detected if not basic then return basic direction
        Vector3 direction = slopeHandler.CheckForSlope().checkForSlope ? slopeHandler.CheckForSlope().slopeMoveDirection : inputHandler.direction;
        return direction.normalized;
    }

    float GetCurrentMaxSpeed()
    {
        // If player is not on ground or is sliding set his speed to air max speed
        if (!isGrounded || isSliding)
        {
            return airMaxSpeed;
        }

        // Return runMaxSpeed if player is running or walkMaxSpeed if is walking
        return inputHandler.isRunning ? runMaxSpeed : walkMaxSpeed;
    }

    #endregion GET / SET FUNCTIONS
}
