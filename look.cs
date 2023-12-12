using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    [Header("Look")]
    [Space]
    [Header("Necessary")]
    [Space]
    [SerializeField] Transform camRoot;
    [SerializeField] Camera cam;
    [SerializeField] Transform camTilt;
    [SerializeField] Transform orientation;
    [SerializeField] Transform player;
    [SerializeField] Transform groundTransform;
    [SerializeField] Transform vaultOffsetTransform;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Move move;
    [Space]
    [Header("Customizable")]
    [Space]
    [Header("Looking")]
    [Space]
    [SerializeField] float lookSensitivity; //5f
    [SerializeField] float aimSensitivity;
    [SerializeField] float maxRotation; //90f[HideInInspector] float mouseX;
    [SerializeField] float cameraTilt;
    [SerializeField] float smothness;
    [HideInInspector] float sensitivity;
    [HideInInspector] public bool aiming;
    [HideInInspector] float mouseY;
    [HideInInspector] float mouseX;
    [HideInInspector] float rotationX;
    [HideInInspector] float rotationY;
    [HideInInspector] float horizontal;
    [HideInInspector] float vertical;
    [HideInInspector] Vector3 vaultOffset;
    [Space]
    [Header("Head bobbing")]
    [Space]
    [SerializeField] float bobFrequency = 10f;
    [SerializeField] float bobAmplitude = 0.05f;
    [HideInInspector] Vector3 cameraOriginalPos;
    [HideInInspector] float timer = 0f;


    void Start()
    {
        // Disabling cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraOriginalPos = camRoot.transform.localPosition;
    }

    void Update()
    {
        // Inputs
        sensitivity = aiming ? aimSensitivity : lookSensitivity;

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        rotationX -= mouseY * sensitivity;
        rotationY += mouseX * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxRotation, maxRotation);
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        // Calculating head tilt
        float localX = camTilt.transform.localRotation.x;
        float localY = camTilt.transform.localRotation.y;
        if(move.sliding || move.crouching){
            camRoot.transform.localPosition = Vector3.Slerp(camRoot.transform.localPosition, new Vector3(0,0.2f,0), smothness * Time.deltaTime);
        }else{
            camRoot.transform.localPosition = Vector3.Slerp(camRoot.transform.localPosition, new Vector3(0,0,0), smothness * Time.deltaTime);
        }
        
        if(move.sliding){
            camTilt.transform.localRotation = Quaternion.Slerp(camTilt.transform.localRotation, Quaternion.Euler(localX, localY, cameraTilt * Input.GetAxisRaw("Horizontal")), 2f * smothness * Time.deltaTime);
        }else{
            camTilt.transform.localRotation = Quaternion.Slerp(camTilt.transform.localRotation, Quaternion.Euler(localX, localY, -cameraTilt * Input.GetAxisRaw("Horizontal") / 2f), 2f * smothness * Time.deltaTime);
        }
        

        // Headbobbing
        float waveslice = Mathf.Sin(timer);
        float totalAxes = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));
        float translateChange = Mathf.Clamp01(totalAxes) * waveslice * bobAmplitude;

        timer = Mathf.Repeat(timer + bobFrequency * Time.deltaTime, Mathf.PI * 2);        
        cam.transform.localPosition = new Vector3(camRoot.localPosition.x, cameraOriginalPos.y + translateChange, 0);

        if(!move.sliding){
            orientation.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
            this.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        }
        camRoot.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
