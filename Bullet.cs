using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Add to bullet prefab
    [Header("Bullet")]
    [Header("Core")]
    [HideInInspector] Rigidbody bulletRigidbody;
    [SerializeField] Transform mainCamera;
    [SerializeField] GameObject bulletModel;
    [SerializeField] GameObject bulletParticleSystem;
    public void Start()
    {
        mainCamera = GameObject.Find("Main Camera").transform;
        ApplyMotionToBullet();
        DestroyBullet(12f, this.gameObject);
        DestroyBullet(12f, bulletParticleSystem);
    }
    public void ApplyMotionToBullet()
    {
        bulletRigidbody = this.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(mainCamera.forward * 100f, ForceMode.VelocityChange);
        bulletRigidbody.velocity = Vector3.ClampMagnitude(bulletRigidbody.velocity, 100f);
    }
    public void OnCollisionEnter(Collision collider)
    {
        if (collider.transform.gameObject.CompareTag("Player"))
        {
            GiveDamage(collider.gameObject, Random.Range(25, 30));
        }
        DestroyBullet(0f, bulletModel);
        bulletRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void DestroyBullet(float timeDoTestroy, GameObject whatToDestroy)
    {
        Destroy(whatToDestroy, timeDoTestroy);
    }
    public void GiveDamage(GameObject colliderGameObject, int damage)
    {
        PlayerLifeFunctions playerLifeFunctionsRef = colliderGameObject.GetComponent<PlayerLifeFunctions>();
        playerLifeFunctionsRef.actualHealth -= damage;
        Debug.Log(playerLifeFunctionsRef.actualHealth);
    }

}
