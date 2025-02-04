using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorAction : InteractionExecution
{
    [Header("Open Door")]
    [Space]
    [Header("References")]
    [SerializeField]
    private Door door;

    [Header("Configuration")]
    [Space]
    [SerializeField]
    private Door.DoorOpenAngle openAngle = Door.DoorOpenAngle.Full;

    [ServerRpc(RequireOwnership = false)]
    public override void ExecuteServerRpc()
    {
        door.SetCurrentDoorAngle(openAngle);
    }
}
