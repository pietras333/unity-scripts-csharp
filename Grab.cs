using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [Header("Grab")]
    [Space]
    [Header("Neccesary")]
    [Space]
    [SerializeField] Transform cam;
    [SerializeField] Transform grabTransform;
    [SerializeField] Material lineMaterial;
    [HideInInspector] RaycastHit grabHit;
    [HideInInspector] bool grabbing;
    [HideInInspector] LineRenderer line;
    [Space]
    [Header("Customizable")]
    [Space]
    [SerializeField, Range(0,10)] float grabStrengh; 
    [SerializeField, Range(0,5)] float grabRange; 
    [SerializeField, Range(0,5)] float maxGrabDistance; 
    [SerializeField, Range(0,50)] float massMultiplier; 

    void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            if(Physics.Raycast(cam.position,cam.forward, out grabHit, grabRange)){
                if(grabHit.transform.gameObject.GetComponent<Rigidbody>() != null && !grabbing){
                    grabStart();
                }
            }
        }
        if(grabbing){
            line.startWidth = 0.25f;
            line.endWidth = 0.05f;
            line.SetPosition(0, grabTransform.transform.position);
            line.SetPosition(1, grabHit.transform.position);
        }
        if(Input.GetKeyUp(KeyCode.E) && grabbing){
            grabEnd();
        }
    }
    void grabStart(){
        grabTransform.gameObject.AddComponent<SpringJoint>();
        SpringJoint grabJoint = grabTransform.GetComponent<SpringJoint>();
        grabJoint.autoConfigureConnectedAnchor = false;
        grabJoint.connectedBody = grabHit.transform.gameObject.GetComponent<Rigidbody>();
        grabJoint.connectedAnchor = grabHit.point;
        grabJoint.damper = 5f;


        grabTransform.transform.gameObject.AddComponent<LineRenderer>();
        line = grabTransform.GetComponent<LineRenderer>();
        line.material = lineMaterial;
        line.numCapVertices = 5;
        
        float distance = Vector3.Distance(grabTransform.position, grabHit.point);
        grabJoint.spring = grabStrengh * massMultiplier;
        grabJoint.tolerance = 1f;
        grabJoint.maxDistance = distance + .25f;
        grabJoint.minDistance = distance;

        grabbing = true;        
    }

    void grabEnd(){
        Destroy(grabTransform.GetComponent<SpringJoint>());
        Destroy(grabHit.transform.gameObject.GetComponent<SpringJoint>());
        Destroy(grabTransform.GetComponent<LineRenderer>());

        grabbing = false;
    }
    
}
