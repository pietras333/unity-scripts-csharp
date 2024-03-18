using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraActivityHandler : NetworkBehaviour
{
    [Header("Camera Activity Handler")]
    [HideInInspector] Camera camera;

    public override void OnNetworkSpawn()
    {
        // If script doesnt belong to player
        if (!IsOwner)
        {
            return;
        }
        // If so...
        // Get camera component
        camera = GetComponent<Camera>();

        // Turn camera on
        camera.enabled = true;
    }
}
