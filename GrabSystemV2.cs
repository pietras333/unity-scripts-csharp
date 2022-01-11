using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSystemV2 : MonoBehaviour
{
    //apply on object holder
    [Header("Grab System")]
    [Header("CORE")]
    [SerializeField] PlayerMovement playerMovementRef;
    [SerializeField] Transform mainCamera;
    [HideInInspector] RaycastHit hit;
    [HideInInspector] Transform objectHolder;
    [HideInInspector] GameObject grabbedItem;
    [Header("BOOOOOOOOOOOOOOOLS")]
    [HideInInspector] bool canGrab;
    [HideInInspector] bool isGrabbing;
    [Header("Customization")]
    [SerializeField] string grabTag;
    [SerializeField] KeyCode grabKey;
    [SerializeField] float grabDistance;
    [SerializeField] float grabSmoothness;
    [HideInInspector] float fixedSmotness;
    private void Update()
    {
        if (Input.GetKey(grabKey))
            Ray();
        if (Input.GetKeyUp(grabKey))
            GrabEnd();

    }
    private void Start()
    {
        objectHolder = this.transform;
        fixedSmotness = grabSmoothness;
    }
    public void Ray()
    {
        if (GrabLogic())
            GrabStart();
    }
    public bool GrabLogic()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, grabDistance))
            grabbedItem = hit.transform.gameObject;
        else grabbedItem = null;
        if (grabbedItem == null) return false;

        if (grabbedItem.CompareTag(grabTag))
            return true;
        else return false;
    }
    public void GrabStart()
    {
        grabbedItem.transform.position = Vector3.Slerp(grabbedItem.transform.position, objectHolder.position, GetSmothness() * Time.deltaTime);
        grabbedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        grabbedItem.GetComponent<Rigidbody>().isKinematic = true;
        grabbedItem.GetComponent<Collider>().isTrigger = true;
    }
    public void GrabEnd()
    {
        grabbedItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        grabbedItem.GetComponent<Rigidbody>().isKinematic = false;
        grabbedItem.GetComponent<Collider>().isTrigger = false;
    }
    public float GetSmothness()
    {
        if (playerMovementRef.moveDirection != Vector3.zero)
            return grabSmoothness + playerMovementRef.actualSpeed;
        else
            return fixedSmotness;
    }
}
