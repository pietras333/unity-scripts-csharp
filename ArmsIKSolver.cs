using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ArmsIkSolver : MonoBehaviour
{
    [Header("Arms IK Solver")]
    [Space]
    [Header("Neccesary")]
    [Space]
    [SerializeField] TwoBoneIKConstraint armLeft;
    [SerializeField] TwoBoneIKConstraint armRight;
    [SerializeField] Transform source;
    [SerializeField] GameObject[] weaponPrefabs;
    [SerializeField] public int weaponIndex;
    [SerializeField] public Transform currentWeapon;
    [HideInInspector] public Quaternion rightHintRot;
    // [HideInInspector] public bool reloading;
    [HideInInspector] public bool reloading;
    [HideInInspector] Vector3 leftTarget;
    [HideInInspector] Transform rightTarget;
    void Update(){
        if(weaponPrefabs.Length <= 0){
            return;
        }
        Transform left = currentWeapon.transform.GetChild(0);
        Transform right = currentWeapon.transform.GetChild(1);
        Transform rightHint = currentWeapon.transform.GetChild(2);
        Transform leftHint = currentWeapon.transform.GetChild(3);
        Transform reloadTarget = currentWeapon.transform.GetChild(5);
        Transform reloadHint = currentWeapon.transform.GetChild(6);

        if(!reloading){
            armLeft.data.target.transform.localPosition = Vector3.Slerp(armLeft.data.target.transform.localPosition, left.transform.localPosition + currentWeapon.transform.localPosition, 10 * Time.deltaTime);
            armLeft.data.target.transform.localRotation = left.transform.localRotation;
        }
        // }else{
        //     armLeft.data.target.transform.localPosition = Vector3.Slerp(armLeft.data.target.transform.localPosition, reloadTarget.transform.localPosition + currentWeapon.transform.localPosition, 10 * Time.deltaTime);
        //     armLeft.data.target.transform.localRotation = reloadTarget.transform.localRotation;
        // }

        armRight.data.target.transform.localPosition = right.transform.localPosition + currentWeapon.transform.localPosition;
        armRight.data.target.transform.localRotation = right.transform.localRotation;

        armLeft.data.hint.transform.localPosition = leftHint.transform.localPosition + currentWeapon.transform.localPosition;
        armLeft.data.hint.transform.localRotation = leftHint.transform.localRotation;

        armRight.data.hint.transform.localPosition = rightHint.transform.localPosition + currentWeapon.transform.localPosition;
        armRight.data.hint.transform.localRotation = rightHint.transform.localRotation;
        
    }

}
