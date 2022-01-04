using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementV3 : MonoBehaviour
{
    [Header("Player Movement")]
    [Header("CORE")]
    [SerializeField] WallRun wallrunRef;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] PhysicMaterial playerPhysicsMaterial;
    [SerializeField] Transform orientation;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform ceilingCheck;
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] LayerMask groundMask;
    [Header("Orientation VARS")]
    [SerializeField] Vector3 moveDirection;
    [SerializeField] public float actualSpeed;
    [SerializeField] public float inputX;
    [SerializeField] float inputY;
    [SerializeField] public bool isGrounded;
    [SerializeField] bool canJump = true;
    [SerializeField] bool isCrouching;
    [Header("Basic Movement Customization")]
    [Header("MOVEMENT")]
    [SerializeField] float colliderNormalHeight = 2;
    [SerializeField] float colliderSlidingCrouchingHeight = 1;
    [SerializeField] float moveDrag = 2.15f;
    [SerializeField] float airDrag = 1.15f;
    [SerializeField] float walkSpeed;
    [SerializeField] float normalWalkSpeed = 2500;
    [SerializeField] float airSpeed = 1250;
    [SerializeField] float maxSpeed;
    [Header("JUMPING")]
    [SerializeField] public KeyCode jumpKey;
    [SerializeField] public float jumpForce = 500;
    [Header("CROUCHING")]
    [SerializeField] KeyCode crouchKey;
    [SerializeField] float crouchSpeed = 1000;
    [SerializeField] float ceilingDetection = 1;

    public void Update()
    {
        GatherInput();
        Vars();
    }
    public void FixedUpdate()
    {
        Movement();
    }
    private void OnCollisionEnter(Collision other)
    {
        Invoke(nameof(_ResetJump), .05f);
    }
    public void GatherInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        isGrounded = Physics.CheckSphere(groundCheck.position, .15f, groundMask);
        moveDirection = inputX * orientation.right + inputY * orientation.forward;
        actualSpeed = playerRigidbody.velocity.magnitude;
        isCrouching = Input.GetKey(crouchKey);
    }
    public void Movement()
    {
        Crouching();
        if (actualSpeed < maxSpeed)
            playerRigidbody.AddForce(moveDirection.normalized * walkSpeed * 1f * Time.fixedDeltaTime, ForceMode.Force);
        if (_ShouldStop())
            playerRigidbody.velocity = Vector3.Slerp(playerRigidbody.velocity, -new Vector3(0, 0, 0), 10 * Time.deltaTime);
        if (_IsReadyToJump()) Jump(Getjumpfrc());
        if (!isGrounded && !wallrunRef.isWallRunningLeft() && !wallrunRef.isWallRunningRight()) playerRigidbody.AddForce(-orientation.up * AirForce() * Time.fixedDeltaTime, ForceMode.Acceleration);
    }
    public void Crouching()
    {
        if (isCrouching || !_CanStandUp())
        {
            playerCollider.height = colliderSlidingCrouchingHeight;
            walkSpeed = crouchSpeed;
        }
        else if (!isCrouching && _CanStandUp())
        {
            playerCollider.height = colliderNormalHeight;
            walkSpeed = normalWalkSpeed;
        }
    }
    public void _ResetJump() => canJump = true;
    public float AirForce()
    {
        return 850 + actualSpeed;
    }
    public bool _IsReadyToJump()
    {
        if (isGrounded && Input.GetKey(jumpKey) && canJump) return true; else return false;
    }
    public bool _ShouldStop()
    {
        if (moveDirection == Vector3.zero && actualSpeed > 1 && isGrounded) return true; else return false;
    }
    public bool _CanStandUp()
    {
        return !(Physics.Raycast(ceilingCheck.position, ceilingCheck.up, ceilingDetection));
    }
    public void Vars()
    {
        if (!isGrounded) walkSpeed = airSpeed; else walkSpeed = normalWalkSpeed;
        if (!isGrounded) maxSpeed = 100; else maxSpeed = 15;
        if (!isGrounded) playerRigidbody.drag = airDrag; else playerRigidbody.drag = moveDrag;
    }
    public float Getjumpfrc() => jumpForce + (actualSpeed / 5);
    public void Jump(float jumpfrc)
    {
        jumpForce = jumpfrc;
        playerRigidbody.AddForce(Vector2.up * jumpForce * 1.2f);
        if (actualSpeed > 15) playerRigidbody.AddForce(Vector2.up * actualSpeed / 10);
        canJump = false;
    }


}
