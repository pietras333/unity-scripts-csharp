using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight")]
    [Header("Core")]
    [SerializeField] GameObject flashlight;
    [HideInInspector] Vector3 vectorOffset;
    [HideInInspector] GameObject goFollow;
    [Header("Customization")]
    [SerializeField] KeyCode flashlightKey;
    [SerializeField] float offsetSpeed;
    public void Start(){
        goFollow = Camera.main.gameObject;
        vectorOffset = flashlight.transform.position - goFollow.transform.position;
    }
    public void Update(){
        FlashlightSystem();
        FlashlightOffset();
    }
    public void FlashlightSystem(){
        if(flashlight.activeSelf == false && Input.GetKeyDown(flashlightKey)){
            flashlight.SetActive(true);
        }else if(flashlight.activeSelf == true && Input.GetKeyDown(flashlightKey)){
            flashlight.SetActive(false);
        }
    }
    public void FlashlightOffset(){
        flashlight.transform.position = goFollow.transform.position + vectorOffset;
        flashlight.transform.rotation = Quaternion.Slerp(flashlight.transform.rotation, goFollow.transform.rotation, offsetSpeed * Time.deltaTime);
    }
}
