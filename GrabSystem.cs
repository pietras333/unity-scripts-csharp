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
  private float dropForce;
  private BoxCollider grabbedObjectCollider;
  private ThrowTeleport throwTeleportRef;
  public float throwForce = 50f;

  public void Update(){
    dropForce = playerRB.velocity.magnitude;
    if(isHolding && Input.GetMouseButtonDown(0)){
      Throw();
      isHolding = false;
    }
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
  
  public void Throw(){
   if(grabbedObject.GetComponent<ThrowTeleport>()){
    throwTeleportRef = grabbedObject.GetComponent<ThrowTeleport>();
    grabbedObjectRB.isKinematic = false;
    grabbedObjectRB.useGravity = true;
    grabbedObject.transform.SetParent(null);
    grabbedObjectCollider = grabbedObject.GetComponent<BoxCollider>();
    grabbedObjectCollider.enabled = true;
    grabbedObjectRB.AddForce(camera.forward * throwForce, ForceMode.Impulse);
    throwTeleportRef.doTeleport = true;
   }else{
    grabbedObjectRB.isKinematic = false;
    grabbedObjectRB.useGravity = true;
    grabbedObject.transform.SetParent(null);
    grabbedObjectCollider = grabbedObject.GetComponent<BoxCollider>();
    grabbedObjectCollider.enabled = true;
    grabbedObjectRB.AddForce(camera.forward * throwForce, ForceMode.Impulse);
   }
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
