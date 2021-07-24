using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlate : MonoBehaviour
{
   private Rigidbody playerRB;
   private GameObject thisGameObject;
   [SerializeField] float upForce = 15f;
   
   private void Start(){
       thisGameObject = this.gameObject;
   }

    private void OnCollisionEnter(Collision collider){
       if(collider.gameObject.CompareTag("Player")){
           playerRB = collider.gameObject.GetComponent<Rigidbody>();
           playerRB.AddForce(thisGameObject.transform.up * upForce, ForceMode.Impulse);
       }else
       {
           return;
       }
   }
}
