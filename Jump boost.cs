using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoost : MonoBehaviour
{
    [SerializeField]
    public RewindTime rewindTimeRef;
    public Player_Movement playerMovementRef;
    public float jumpBoost = 750f;

    private void OnCollisionEnter(Collision collision)
    {
        rewindTimeRef = collision.gameObject.GetComponent<RewindTime>();
        playerMovementRef = collision.gameObject.GetComponent<Player_Movement>();
        if (collision.gameObject.CompareTag("Player"))
        {
            if (rewindTimeRef.isRewind)
            {
                StartIncreaseJump();
                Invoke("StopIncreaseJump", 6f);
            }
        }
    }

    void StartIncreaseJump()
    {
        playerMovementRef.jumpForce = jumpBoost;
    }

    void StopIncreaseJump()
    {
        playerMovementRef.jumpForce = 300f;
    }
}
