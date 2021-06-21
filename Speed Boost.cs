using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField]
    public RewindTime rewindTimeRef;
    public Player_Movement playerMovementRef;
    public float speedBoostMaxSpeed = 30f;
    public float speedBoostMoveSpeed = 5000f;

    private void OnCollisionEnter(Collision collision)
    { 
        rewindTimeRef = collision.gameObject.GetComponent<RewindTime>();
        playerMovementRef = collision.gameObject.GetComponent<Player_Movement>();
        if (collision.gameObject.CompareTag("Player"))
        {
            if (rewindTimeRef.isRewind)
            {
                IncreaseSpeed();
                Invoke("DecreaseSpeed",6f);
            }
        }
    }

    void IncreaseSpeed()
    {
        playerMovementRef.moveSpeed = speedBoostMoveSpeed;
        playerMovementRef.maxSpeed = speedBoostMaxSpeed;
    }

    void DecreaseSpeed()
    {
        playerMovementRef.moveSpeed = 4500f;
        playerMovementRef.maxSpeed = 20f;
    }
}
