using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDAnimator : MonoBehaviour
{
    [Header("Hud Animator")]
    [Space]
    [Header("Transforms")]
    [SerializeField] Move move;
    [SerializeField] Transform canvasTarget;
    [SerializeField] Animator hudAnimator;
    [SerializeField] TextMeshProUGUI informationMessage;
    [Space]
    [Header("States")]
    [Space]
    [HideInInspector] public bool visible;

    void Update(){
        Vector3 velocity = move.velocity;
        velocity = new Vector3(Mathf.Clamp(velocity.x, 0.1f, 0.25f), Mathf.Clamp(velocity.y, 0.1f, 0.25f), Mathf.Clamp(velocity.z, 0.1f, 0.25f));
        this.transform.position = Vector3.SmoothDamp(this.transform.position, canvasTarget.transform.position, ref velocity, 5f * Time.deltaTime);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, canvasTarget.transform.rotation, 5f * Time.deltaTime);
        hudAnimator.SetBool("visible", visible);
    }

    public void hideInformationPopup(){
        hudAnimator.SetBool("information", false);
    }

    public void informationPopup(string message){
        print("information showed");
        informationMessage.text = message;
        hudAnimator.SetBool("information", true);
        Invoke("hideInformationPopup", 2f);
    }
}
