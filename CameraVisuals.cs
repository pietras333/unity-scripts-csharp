using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class CameraVisuals : MonoBehaviour
{
    // U will need EZCameraShake asset for this available on GitHub and rigidbody based movement
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float actualSpeed;
    [SerializeField] PostProcessProfile postProcessProfile;
    [HideInInspector] ChromaticAberration chromaticAberration;

    void Start(){
        chromaticAberration = postProcessProfile.GetSetting<ChromaticAberration>();
    }
    void Update()
    {
        actualSpeed = rigidbody.velocity.magnitude;
        chromaticAberration.intensity.value = actualSpeed/10;     

        if(actualSpeed> 4f && actualSpeed < 12.9f)
         CameraShaker.Instance.ShakeOnce(.3f,.3f,.1f,1f);

        if(actualSpeed > 13f)
         CameraShaker.Instance.ShakeOnce(.55f,.55f,.1f,1f);        
    }
}
