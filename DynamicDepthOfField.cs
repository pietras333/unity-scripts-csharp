using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; 
using UnityEngine.Rendering.Universal; 

public class DynamicDepthOfField : MonoBehaviour
{
    [Header("Dynamic Depth Of Field")]
    [Space]
    [Header("Neccesary")]
    [SerializeField] Transform cam;
    [SerializeField] Volume volume;
    [Space]
    [Header("Customizable")]
    [SerializeField, Range(0, 10)] float focusSpeed; 

    void Update(){
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f)){
            if(volume.profile.TryGet(out DepthOfField depthOfField)){
                depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, Vector3.Distance(cam.transform.position, hit.point), focusSpeed * Time.deltaTime);
            }   
        }

    }

}
