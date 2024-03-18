using UnityEngine;

public class BallControl : MonoBehaviour
{
    [Header("Ball Control")]
    [Space]
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] KickHandler kickHandler;
    [Space]
    [Header("Configuration")]
    [Space]
    [Header("Detection")]
    [SerializeField] float detectionVerticalOffset = 0.75f;
    [SerializeField] float ballDetectionRange = 0.75f;
    [SerializeField] LayerMask ballLayer;
    [Space]
    [Header("States")]
    [SerializeField] public bool canControlBall = true;
    [SerializeField] public bool isDribbling;

    void OnDrawGizmos()
    {
        DebugBallControlRange();
    }

    void DebugBallControlRange()
    {
        Gizmos.color = Color.green;
        Vector3 detectionPosition = transform.position - new Vector3(0, detectionVerticalOffset, 0) + transform.forward;
        Gizmos.DrawSphere(detectionPosition, ballDetectionRange);
    }

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (!rb)
        {
            Debug.LogError("One or more references are missing in the BallControl script.", gameObject);
            return;
        }
    }

    void FixedUpdate()
    {
        DetectAndControlBall();
    }

    void DetectAndControlBall()
    {
        Vector3 checkBallPosition = transform.position - new Vector3(0, detectionVerticalOffset, 0) + transform.forward;
        Collider[] colliders = Physics.OverlapSphere(checkBallPosition, ballDetectionRange, ballLayer);

        if (colliders.Length < 1)
        {
            isDribbling = false;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            GameObject ball = colliders[i].gameObject;
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

            if (kickHandler.isKicking)
            {
                Debug.LogWarning("Cant dribble ball when kicking", gameObject);
                return;
            }

            if ((!ballRigidbody || !canControlBall) && !kickHandler.isKicking)
            {
                Debug.LogWarning("Ball Rigidbody not found or you can't controll the ball!", gameObject);
                return;
            }
            isDribbling = true;
            ball.transform.position = Vector3.Slerp(ball.transform.position, transform.position - new Vector3(0, detectionVerticalOffset, 0) + transform.forward, 5 * Time.deltaTime);
            ballRigidbody.velocity = rb.velocity;
        }
    }
}
