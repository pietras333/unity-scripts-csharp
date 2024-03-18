using System.Collections;
using UnityEngine;

public class TackleHandler : MonoBehaviour
{
    [Header("TackleHandler Handler")]
    [Space]
    [Header("References")]
    [SerializeField] Movement movement;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [Space]
    [Header("Configuration")]
    [SerializeField] KeyCode tackleKey = KeyCode.G;
    [SerializeField] public float tackleCooldown = 0.25f;
    [SerializeField] public float tackleForce = 7f;
    [SerializeField, Range(0.5f, 3)] float tackleForceMultiplier = 0.7f;
    [SerializeField] public float tackleForceAccumulator = 0.5f;
    [SerializeField] string playerTag = "Player";
    [Header("Detection")]
    [SerializeField] Vector3 tackleDirection;
    [SerializeField] LayerMask ballLayer;
    [Space]
    [Header("States")]
    [SerializeField] float currentTackleForce;
    [SerializeField] public bool isTackling;
    [SerializeField] bool canShowTackle;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (!movement || !animator || !movement)
        {
            Debug.LogError("One or more references are missing in the TackleHandler script.", gameObject);
            return;
        }
    }

    void Update()
    {
        CheckTacklingState();

        UpdateTacklingState();

        ClampTackleForce();

        animator.SetBool("isTackling", canShowTackle);
    }

    public void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == playerTag)
        {
            if (!collider.gameObject.GetComponent<TackledStateController>())
            {
                Debug.LogError("Tackled State Controller not found!");
                return;
            }
            TackledStateController tackledStateController = collider.gameObject.GetComponent<TackledStateController>();
            tackledStateController.Tackled();
        }
    }

    void CheckTacklingState()
    {
        if (Input.GetKeyDown(tackleKey) && !isTackling)
        {
            isTackling = true;
            StartCoroutine("TackleForceIncrementation");
        }
        if (Input.GetKeyUp(tackleKey) && isTackling)
        {
            movement.canMove = false;
            canShowTackle = true;
            tackleDirection = movement.lastDirection;
            StopCoroutine("TackleForceIncrementation");
            StartCoroutine("TackleForceDecrease");
        }
    }

    void UpdateTacklingState()
    {
        if (canShowTackle)
        {
            rb.AddForce(transform.forward * currentTackleForce * tackleForceMultiplier, ForceMode.Impulse);
        }

    }

    void StopTackling()
    {
        isTackling = false;
        movement.canMove = true;
        StartCoroutine("TackleForceDecrease");
    }

    IEnumerator TackleForceIncrementation()
    {
        while (currentTackleForce < tackleForce)
        {
            currentTackleForce += tackleForceAccumulator;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    IEnumerator TackleForceDecrease()
    {
        while (currentTackleForce > 0)
        {
            currentTackleForce -= tackleForceAccumulator * 0.5f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        canShowTackle = false;
        Invoke("StopTackling", tackleCooldown);
    }

    void ClampTackleForce()
    {
        currentTackleForce = Mathf.Clamp(currentTackleForce, 0, tackleForce);
    }

}
