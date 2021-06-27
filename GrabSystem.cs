using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{ 
  public Transform camera;
  public GrapplingGun grapplingGunRef;
  public RotateGun rotateGunRef;
  public Transform grabPosition;
  private bool isHolding = false;
  private Rigidbody grabbedObjectRB;
  public GameObject grabbedObject;
  private float grabRange = 5f;
  private float dropForce = 5f;
  private BoxCollider grabbedObjectCollider;

  public void Update(){
    // WHEN PRESSED E
    if(Input.GetKeyDown("e") && !isHolding){
      CheckItem();
      // IF CHECKED ITEM IS GRABABLE
      if(grabbedObject.CompareTag("Grabable")){
        // WE GRAB
        PickUp();
        isHolding = true;
      }else{
        return;
      }
      // IF WE PRESSED E AND IS HOLDING
    }else if(Input.GetKeyDown("e") && isHolding){
      // WE DROP
      Drop();
      isHolding = false;
    }

    if(isHolding){
      grapplingGunRef.enabled = true;
      rotateGunRef.enabled = true;
    }else{
      grapplingGunRef.enabled = false;
      rotateGunRef.enabled = false;
    }

   



  }

  public void PickUp(){
    grabbedObjectRB = grabbedObject.GetComponent<Rigidbody>();
    grabbedObject.transform.SetParent(grabPosition);
    grabbedObject.transform.localPosition = new Vector3(1f,-1f,0.5f);
    grabbedObject.transform.localRotation = Quaternion.Euler(0f,0f,0f);
    grabbedObjectRB.useGravity = false;
    grabbedObjectRB.isKinematic = true;  
    grabbedObjectCollider = grabbedObject.GetComponent<BoxCollider>();
    grabbedObjectCollider.enabled = false;
  }

  public void Drop(){
    grabbedObjectRB.isKinematic = false;
    grabbedObjectRB.useGravity = true;
    grabbedObject.transform.SetParent(null);
    grabbedObjectCollider = grabbedObject.GetComponent<BoxCollider>();
    grabbedObjectCollider.enabled = true;
    grabbedObjectRB.AddForce(camera.forward * dropForce, ForceMode.Impulse);
  }

  public void CheckItem(){
    RaycastHit hit;
    if(Physics.Raycast(camera.position, camera.forward, out hit, grabRange)){
      grabbedObject = hit.transform.gameObject;      
    }
  }

}
