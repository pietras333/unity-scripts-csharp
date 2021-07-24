using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float minYLevel;
    [SerializeField] float explosionForce;
    [SerializeField] float radius;
    [HideInInspector] public bool doExplode = false;
    private Vector3 objectPosition;
    private GameObject thisGameObject;
    private Rigidbody rigidbody;
    public void Start(){
        thisGameObject = this.gameObject;
        rigidbody = player.GetComponent<Rigidbody>();
        objectPosition = new Vector3(thisGameObject.transform.position.x, thisGameObject.transform.position.y,thisGameObject.transform.position.z);
    }
    public void Update(){
        if(thisGameObject.transform.position.y < minYLevel){
          thisGameObject.transform.position = objectPosition;
        }
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

