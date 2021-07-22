using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // use my grab system for this
    [SerializeField] GameObject player;
    [SerializeField] float explosionForce;
    [SerializeField] float radius;
    [HideInInspector] public bool doExplode = false;
    private GameObject thisGameObject;
    private Rigidbody rigidbody;
    public void Start(){
        thisGameObject = this.gameObject;
        rigidbody = player.GetComponent<Rigidbody>();
    }
    public void OnCollisionEnter(Collision collider){
        if(doExplode){
            AddForce();
            Destroy(thisGameObject);
        }
    }
    private void AddForce(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider col in colliders)
        { 
           rigidbody.AddExplosionForce(explosionForce, transform.position, radius);
        }
    }
}
