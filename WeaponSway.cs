using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    /// Add to weapon model
    /// Setting I use
    /// Rotation multiplier 30
    /// Position multiplier 0.15
    /// Smoothness 3
    [Header("Weapon Sway")]
    [Header("Core")]
    [SerializeField] PlayerGun playerGunRef;
    [SerializeField] Transform mainCamera;
    [SerializeField] RaycastHit hit;
    [SerializeField] float mouseX;
    [SerializeField] float mouseY;
    [SerializeField] Quaternion rotationX;
    [SerializeField] Quaternion rotationY;
    [SerializeField] Quaternion targetRotation;
    [SerializeField] float positionX;
    [SerializeField] float positionY;
    [SerializeField] Vector3 targetPosition;
    [Header("Customization")]
    [SerializeField] float rotationMultiplier;
    [SerializeField] float positionMultiplier;
    [SerializeField] float smoothness;
    public void Update()
    {
        GatherUserInput();
        CalculateTargetPositionAndRotation();
        if (!playerGunRef.isScoping)
            TransformPositionAndRotation();
        else
            AllignRotationToCrosshair();

    }
    public void AllignRotationToCrosshair()
    {
        Physics.Raycast(mainCamera.position, mainCamera.forward, out hit);
        transform.LookAt(hit.point);
    }
    public void GatherUserInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
    }
    public void CalculateTargetPositionAndRotation()
    {
        rotationX = Quaternion.AngleAxis((-mouseY * rotationMultiplier), Vector3.right);
        rotationY = Quaternion.AngleAxis((mouseX * rotationMultiplier), Vector3.up);
        targetRotation = rotationX * rotationY;
        positionX = (-mouseY * positionMultiplier);
        positionY = (mouseX * positionMultiplier);
        targetPosition = new Vector3(positionX, positionY, 0f);
    }
    public void TransformPositionAndRotation()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothness * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, (smoothness / 2) * Time.deltaTime);
    }

}
