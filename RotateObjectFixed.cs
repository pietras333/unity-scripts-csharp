using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectFixed : MonoBehaviour
{
    [Header("Rotate Object")]
    [Header("CORE")]
    [SerializeField] KeyCode rotateKey;
    [SerializeField] CameraMovement camMoveRef;
    [SerializeField] GrabSys grabSysRef;
    [Header("blablabla")]
    [SerializeField] float smothness;
    [SerializeField] float rotx;
    [SerializeField] float roty;
    private void Update()
    {
        if (WantsToRotate() && CanRotate())
        {
            GatherInput();
            ApplyObjectRotation();
        }
    }
    public void GatherInput()
    {
        rotx -= Input.GetAxisRaw("Mouse Y") * camMoveRef.sensitivity;
        roty += Input.GetAxisRaw("Mouse X") * camMoveRef.sensitivity;
    }
    public void ApplyObjectRotation()
    {
        Quaternion target = Quaternion.Euler(rotx, roty, 0);
        Quaternion origin = GetObject().transform.rotation;
        GetObject().transform.rotation = Quaternion.Slerp(origin, target, smothness * Time.deltaTime);
    }
    public GameObject GetObject()
    {
        if (grabSysRef.grabbedItem.GetComponent<Rigidbody>() != null)
            return grabSysRef.grabbedItem;
        else return null;
    }
    public bool WantsToRotate()
    {
        if (Input.GetKey(rotateKey))
            return true;
        else return false;
    }
    public bool CanRotate()
    {
        if (GetObject() != null)
            return true;
        else return false;
    }
}
