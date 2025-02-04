using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetAppActivityUI : UIAction
{
    public enum AppActivity
    {
        turnOff,
        turnOn
    };

    [Header("Set App Activity UI")]
    [Space]
    [SerializeField]
    private GameObject appUI;

    [SerializeField]
    private AppActivity appActivity;

    public override void ExecuteAction()
    {
        if (IsServer)
        {
            SetAppActivityClientRpc(appActivity == AppActivity.turnOn);
        }
        else
        {
            SetAppActivityServerRpc(appActivity == AppActivity.turnOn);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetAppActivityServerRpc(bool activity)
    {
        SetAppActivityClientRpc(activity);
    }

    [ClientRpc]
    private void SetAppActivityClientRpc(bool activity)
    {
        appUI.SetActive(activity);
    }
}
