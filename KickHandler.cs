using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class KickHandler : MonoBehaviour
{
    [Header("Kick Handler")]
    [Space]
    [Header("References")]
    [SerializeField] Movement movement;
    [SerializeField] Animator animator;
    [SerializeField] BallControl ballControl;
    [SerializeField] Transform orientation;
    [Space]
    [Header("Configuration")]
    [SerializeField] KeyCode kickKey = KeyCode.F;
    [SerializeField] public float kickCooldown = 0.25f;
    [HideInInspector] float kickCooldownHalf;
    [SerializeField] public float kickForce = 10f;
    [SerializeField, Range(1, 2)] float kickForceMultiplier = 2f;
    [SerializeField] public float kickForceAccumulator = 0.65f;
    [Space]
    [Header("Up Force")]
    [SerializeField] KeyCode upForceKey = KeyCode.Space;
    [SerializeField] public float upForceAccumulator = 0.5f;
    [SerializeField] public float upForce = 10f;
    [SerializeField] float currentUpForce;
    [Space]
    [Header("Detection")]
    [SerializeField] LayerMask ballLayer;
    [SerializeField] float kickDetectionRange = 0.85f;
    [Space]
    [Header("Ground Kick")]
    [SerializeField] float groundKickDetectionOffset = 0.5f;
    [Space]
    [Header("Volley Kick")]
    [SerializeField] float volleyKickDetectionOffset = 0f;
    [Space]
    [Header("States")]
    [SerializeField] float currentKickForce;
    [SerializeField] public bool isKicking;
    [SerializeField] bool canShowKick;
    [SerializeField] public bool isUpForce;

    void Start()
    {
        kickCooldownHalf = kickCooldown * 0.5f;
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (!movement || !ballControl || !animator)
        {
            Debug.LogError("One or more references are missing in the KickHandler script.", gameObject);
            return;
        }
    }

    void Update()
    {
        CheckKickingState();

        CheckUpForceState();

        ClampForces();

        animator.SetBool("isKicking", canShowKick);
    }

    void CheckKickingState()
    {
        if (Input.GetKeyDown(kickKey) && !isKicking)
        {
            isKicking = true;
            StartCoroutine("KickForceIncrementation");
        }
        if (Input.GetKeyUp(kickKey) && isKicking)
        {
            ballControl.canControlBall = false;
            movement.canMove = false;
            canShowKick = true;

            StopCoroutine("KickForceIncrementation");

            Invoke("HandleKick", kickCooldownHalf);
            Invoke("StopKicking", kickCooldown);
            Invoke("AllowBallControl", kickCooldown);
        }
    }

    void StopKicking()
    {
        isKicking = false;
        canShowKick = false;
        movement.canMove = true;
    }

    void HandleKick()
    {
        HandleBasicKick();
    }

    void HandleBasicKick()
    {
        Collider[] capsuleColliders = Physics.OverlapCapsule(transform.position + movement.lastDirection - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f, ballLayer);
        for (int i = 0; i < capsuleColliders.Length; i++)
        {
            Rigidbody ballRb = capsuleColliders[i].GetComponent<Rigidbody>();
            BallData ballData = capsuleColliders[i].GetComponent<BallData>();
            if (!ballRb || !ballData)
            {
                Debug.LogWarning("Ball rigidbody and/or BallData script not found on kick!", gameObject);
            }
            GameObject ball = capsuleColliders[i].transform.gameObject;

            Vector3 ballPosition = ball.transform.position;
            Vector3 thisPosition = transform.position;

            float heightDifference = math.abs(ballPosition.y - thisPosition.y);
            animator.SetFloat("BallPosY", heightDifference);
            Debug.Log("heightDifference, " + heightDifference);

            if (!isUpForce)
            {
                ballRb.AddForce(orientation.transform.forward * currentKickForce * kickForceMultiplier, ForceMode.Impulse);
                return;
            }
            ballRb.AddForce(this.transform.forward * currentKickForce * kickForceMultiplier + Vector3.up * currentUpForce, ForceMode.Impulse);
            ballData.lastTouchedBy = transform.gameObject;
        }
        currentUpForce = 0;
        currentKickForce = 0;
    }

    IEnumerator KickForceIncrementation()
    {
        while (currentKickForce < kickForce)
        {
            currentKickForce += kickForceAccumulator;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    void AllowBallControl()
    {
        ballControl.canControlBall = true;
    }

    void CheckUpForceState()
    {
        if (Input.GetKeyDown(upForceKey) && !isUpForce)
        {
            isUpForce = true;
            StartCoroutine("upForceIncrementation");
        }
        if (Input.GetKeyUp(upForceKey) && isUpForce)
        {
            StopCoroutine("upForceIncrementation");
            StopUpForce();
        }
    }

    IEnumerator upForceIncrementation()
    {
        while (currentUpForce < upForce)
        {
            currentUpForce += upForceAccumulator;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    void StopUpForce()
    {
        currentUpForce = 0;
        isUpForce = false;
    }

    void ClampForces()
    {
        currentUpForce = Mathf.Clamp(currentUpForce, 0, upForce);
        currentKickForce = Mathf.Clamp(currentKickForce, 0, kickForce);
    }
}
