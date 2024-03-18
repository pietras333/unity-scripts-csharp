using UnityEngine;

public class BallData : MonoBehaviour
{
    [Header("Ball Data")]
    [Space]
    [Header("References")]
    [SerializeField] Rigidbody ballRb;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [Space]
    [Header("Configuration")]
    [SerializeField] float groundCheckRange = 0.25f;
    [SerializeField] float groundedMaxSpeed = 13f;
    [SerializeField] float inAirMaxSpeed = 15f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float playerCheckRange = 0.65f;
    [SerializeField] float ballMaxSpeed;
    [Space]
    [Header("States")]
    [SerializeField] bool isGrounded;
    [HideInInspector] public GameObject lastTouchedBy;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (!ballRb || !groundCheck)
        {
            Debug.LogError("One or more references are missing in the BallData script.", gameObject);
            return;
        }

    }

    void Update()
    {
        HandlePlayerRecognition();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRange, groundLayer);
        ballMaxSpeed = isGrounded ? groundedMaxSpeed : inAirMaxSpeed;
    }

    void HandlePlayerRecognition()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, playerCheckRange, playerLayer);
        if (players.Length <= 0)
        {
            return;
        }
        lastTouchedBy = players[players.Length - 1].gameObject;
    }

    void FixedUpdate()
    {
        ClampBallVelocity();
    }

    void ClampBallVelocity()
    {
        ballRb.velocity = Vector3.ClampMagnitude(ballRb.velocity, ballMaxSpeed);
    }
}
