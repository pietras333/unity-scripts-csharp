using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BetaMenu : MonoBehaviour
{
    public TMP_InputField joinCodeInput;
    public string joinCode;

    public async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(
                allocation.AllocationId
            );

            PlayerPrefs.SetString("JoinCode", relayJoinCode.ToString());
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager
                .Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby-beta", LoadSceneMode.Single);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        GameObject client = NetworkManager
            .Singleton
            .SpawnManager
            .SpawnedObjects[clientId]
            .gameObject;

        // client.GetComponentInChildren<CodeSetter>().SetCode(PlayerPrefs.GetString("JoinCode"));
    }

    public async void JoinRelay()
    {
        joinCode = joinCodeInput.text;
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(
                joinCode
            );
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager
                .Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
