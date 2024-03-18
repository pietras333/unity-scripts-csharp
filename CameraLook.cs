using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Camera Lock")]
    [Space]
    [Header("References")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform camera;
    [SerializeField] Transform orientation;
    [Space]
    [Header("Configuration")]
    [SerializeField] float sensitivity = 5f;
    [SerializeField] float followSpeed = 10f;
    [SerializeField] float offset = 8f;
    [SerializeField] LayerMask clipLayer;
    [SerializeField] float maxRotationX = 60f;
    [Space]
    [Header("States")]
    [HideInInspector] float mouseX;
    [HideInInspector] float mouseY;
    [HideInInspector] float rotationX;
    [HideInInspector] float rotationY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleInput();
        HandleObjectRotation();
    }

    void HandleInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        rotationX -= mouseY * sensitivity;
        rotationY += mouseX * sensitivity;
        rotationX = math.clamp(rotationX, -maxRotationX, maxRotationX);
    }

    void HandleObjectRotation()
    {
        orientation.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        cameraHolder.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
a
        // Raycast to detect obstacles
        RaycastHit hit;
        Vector3 targetPosition = cameraHolder.transform.position - cameraHolder.transform.forward * offset;
        if (Physics.Raycast(cameraHolder.transform.position, -cameraHolder.transform.forward, out hit, offset, clipLayer))
        {
            // If the ray hits something, adjust the target position
            targetPosition = hit.point + camera.transform.forward;
        }

        // Smoothly move the camera to the target position
        camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, 1000f * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(orientation.transform.position, orientation.transform.position + orientation.transform.forward);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(orientation.transform.position, orientation.transform.position + orientation.transform.right);
    }
}
