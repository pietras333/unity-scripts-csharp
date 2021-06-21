using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject explosionParticle;
    public SlowMotion slowMotionRef;
    public Rigidbody playerRb;
    public float radius = 1f;
    public float power = 800f;
    public bool canSlowMo;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject explosion = Instantiate(explosionParticle, transform.position, transform.rotation);
            slowMotionRef.DoSlowMotion();
            AddForce();
            Destroy(explosionParticle, 3f);
            Destroy(gameObject);

        }
    }
    private void FixedUpdate()
    {
        canSlowMo = false;
    }



    void AddForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider col in colliders)
        { 

            playerRb.AddExplosionForce(power, transform.position, radius);
        }
    }
}
