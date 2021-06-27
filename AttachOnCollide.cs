using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToObject : MonoBehaviour
{
    public GameObject gameObjectToTransform;
    public GameObject showObject;
    private GameObject collidedGameObject;
    private Transform attachPlaceTransform;
    private BoxCollider attachObjectCollider;
    private Rigidbody attachedObjectRigidbody;
    private bool isInPlace = false;

    private void Update(){
        if(isInPlace){
            DestroyAndShow();
        }
    }

    private void Awake(){
        attachObjectCollider = gameObject.GetComponent<BoxCollider>();
        attachPlaceTransform = this.transform;
    }


    private void OnCollisionEnter(Collision collider){
       collidedGameObject = collider.gameObject;
       if(collidedGameObject.CompareTag("Player") || collidedGameObject.CompareTag("Untagged")){
           return;
       }else{
           collidedGameObject.transform.SetParent(attachPlaceTransform);
           collidedGameObject.transform.localPosition = new Vector3(0f,-0.005f,0f);
           collidedGameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f,90f,0f));
           collidedGameObject.transform.localScale = new Vector3(0.03f,0.03f,0.03f);
           attachedObjectRigidbody = collidedGameObject.GetComponent<Rigidbody>();
           attachedObjectRigidbody.useGravity = false;
           attachedObjectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
           isInPlace = true;
       }

    }


    private void DestroyAndShow(){
        Destroy(gameObjectToTransform, 5f);
        Invoke("showObjectVoid", 5f);
        
    }
    
    private void showObjectVoid(){
        showObject.SetActive(true);
    }
}
