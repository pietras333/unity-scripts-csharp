using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // NEED TO BE FIXED 1!!112
    [Header("Rotate Object")]
    [Header("CORE")]
    [SerializeField] KeyCode rotateKey;
    [SerializeField] CameraMovement camMoveRef;
    [SerializeField] GrabSys grabSysRef;
    [SerializeField] public bool canRotate;
    [Header("blablabla")]
    [HideInInspector] float mouseX;
    [HideInInspector] float mouseY;
    [HideInInspector] float rotX;
    [HideInInspector] float rotY;
    private void Update()
    {
        GetInput();
        if (canRotate)
            ApplyRotation();
    }
    public void GetInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        rotX -= mouseY * camMoveRef.sensitivity;
        rotY += mouseX * camMoveRef.sensitivity;
    }
    public void ApplyRotation()
    {
        if (Input.GetKey(rotateKey))
        {
            GameObject obj = grabSysRef.grabbedItem;
            if (obj.GetComponent<Rigidbody>() != null)
                obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, Quaternion.Euler(rotX, rotY, 0), 7 * Time.deltaTime);
        }
    }

}
