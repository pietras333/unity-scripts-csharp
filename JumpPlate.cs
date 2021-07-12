using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlate : MonoBehaviour
{
   // Simple object that throws you up if youre using rigidbody based movement ofc
   private Rigidbody playerRB;
   [SerializeField] float upForce = 15f;
   
   private void OnCollisionEnter(Collision collider){
       if(collider.gameObject.CompareTag("Player")){
           playerRB = collider.gameObject.GetComponent<Rigidbody>();
           playerRB.AddForce(Vector3.up * upForce, ForceMode.Impulse);
       }else
       {
           return;
       }
   }
}
