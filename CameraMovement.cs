using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Movement")]
    [Header("CORE")]
    [SerializeField] WallRun wallRunRef;
    [SerializeField] Transform player;
    [SerializeField] Transform cameraParent;
    [SerializeField] Transform orientation;
    [Header("Orientation VARS")]
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;
    [SerializeField] float rotationX;
    [SerializeField] float rotationY;
    [Header("Customization")]
    [HideInInspector] public float cameraTilt;
    [SerializeField] float sensitivity;
    [SerializeField] float maxRotationX;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Update()
    {
        GatherInput();
        ApplyCameraRotation();

    }
    public void GatherInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        rotationX -= mouseY * sensitivity;
        rotationY += mouseX * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxRotationX, maxRotationX);
    }
    public void ApplyCameraRotation()
    {
        orientation.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        player.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        cameraParent.transform.rotation = Quaternion.Euler(rotationX, rotationY, cameraTilt);
    }

}
