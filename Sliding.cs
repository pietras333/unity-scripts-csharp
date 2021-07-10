using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
  // this is only for RigidBody based movement that's using movementMultilpier float to move
  float slideScale

 
 void StartSliding(){
      isSliding = true;
      movementMultiplier = 0f;
      Vector3 slideScale = new Vector3(1f,0.5f,1f);
      gameObject.transform.localScale = slideScale;
      rb.AddForce(cameraTransform.forward * slideForce, ForceMode.Impulse);
      rb.AddForce(cameraTransform.forward * -5f, ForceMode.Impulse);
    }

  void StopSliding(){
      isSliding = false;
      movementMultiplier = 10f;
      Vector3 normalScale = new Vector3(1f,1f,1f);
      gameObject.transform.localScale = normalScale;
  } 
   
}
