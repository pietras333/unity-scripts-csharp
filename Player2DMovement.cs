using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [Header("Core")]
    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] Transform groundCheck;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Vector2 moveDirectionX;
    [SerializeField] Vector2 moveDirectionY;
    [SerializeField] float inputX;
    [SerializeField] float inputY;
    [Header("Customization")]
    [SerializeField] float actualSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float jumpForce; 

    public void Update(){
        PlayerInput();
        PlayerMove();
        ControlSpeed();
    }
    public void PlayerInput(){
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        moveDirectionX = new Vector2(inputX, 0f);
        moveDirectionY = new Vector2(0f, inputY);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundMask);
        actualSpeed = playerRigidbody.velocity.magnitude;
    }
    public void PlayerMove(){
        playerRigidbody.AddForce(moveDirectionX * moveSpeed, ForceMode2D.Force);
        if(isGrounded)
         playerRigidbody.AddForce(moveDirectionY * jumpForce, ForceMode2D.Impulse);
    }
    public void ControlSpeed(){
       playerRigidbody.velocity = Vector2.ClampMagnitude(playerRigidbody.velocity, maxMoveSpeed); 
    }
}
