using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.UI;
using TMPro;

public class TestRelay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputFld;
    [SerializeField] TextMeshProUGUI codeDisplay;
    [SerializeField] string userCodeInput;

    void Update()
    {
        userCodeInput = inputFld.text.ToString();
        userCodeInput = userCodeInput.Substring(0, 6);
        Debug.Log(userCodeInput);
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join code: " + code);

            codeDisplay.text = code;

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException error)
        {
            Debug.Log(error);
        }
    }


    public async void JoinRelay()
    {
        try
        {
            Debug.Log("Joining relay with " + userCodeInput);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(userCodeInput);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException error)
        {
            Debug.Log(error);
        }
    }
}
