using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [Header("Core")]
    [SerializeField] Rigidbody playerRB;
    [SerializeField] Transform orientation; 
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform playerModel;
    [SerializeField] Transform cameraHolder;
    [Header("Movement Variables")]
    [SerializeField] float actualSpeed;
    [SerializeField] float horizontalAxis;
    [SerializeField] float verticalAxis;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector3 crouchScale;
    [SerializeField] Vector3 normalScale;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isSprinting;
    [SerializeField] bool isJumping;
    [SerializeField] bool isCrouching;
    [Header("Movement Customization")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] KeyCode jumpKey;
    [SerializeField] KeyCode sprintKey;
    [SerializeField] KeyCode crouchKey;
    [SerializeField] float moveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float maxSprintSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float maxCrouchSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float airSpeed;
    public void Update(){
        UserInput();
        UserMove();
        ControlSpeed();
    }
    public void UserInput(){
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(sprintKey);
        isJumping = Input.GetKey(jumpKey);
        isCrouching = Input.GetKey(crouchKey);
        moveDirection = horizontalAxis * orientation.right + verticalAxis * orientation.forward;
        actualSpeed = playerRB.velocity.magnitude;
    }
    public void UserMove(){
        if(isGrounded && !isSprinting)
         playerRB.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        if(isGrounded && isSprinting)
         playerRB.AddForce(moveDirection * sprintSpeed, ForceMode.Acceleration);  
        if(isGrounded && isJumping)
         playerRB.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
        if(!isGrounded)
         playerRB.AddForce(moveDirection * airSpeed, ForceMode.Acceleration);  
        if(isCrouching)
         StartCrouching();
        else
         StopCrouching(); 
    }
    public void ControlSpeed(){
        if(!isSprinting && !isCrouching || isCrouching && !isGrounded)
         playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, maxMoveSpeed);
        if(isSprinting && !isCrouching)
         playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, maxSprintSpeed);
        if(isCrouching && isGrounded)
         playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, maxCrouchSpeed); 
    }
    public void StartCrouching(){
        playerModel.transform.localScale = crouchScale;
        cameraHolder.transform.localPosition = new Vector3(0f,1.3f,0f);
        playerRB.AddForce(moveDirection * crouchSpeed, ForceMode.Acceleration);
    }
    public void StopCrouching(){
        playerModel.transform.localScale = normalScale;
        cameraHolder.transform.localPosition = new Vector3(0f,1.7f,0f);
    }
}
