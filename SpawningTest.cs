using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawningTest : NetworkBehaviour
{
    public GameObject spawn;
   
    void Update()
    {
        if(IsOwner && Input.GetKey(KeyCode.L)){
            spawnServerRpc();
        }
    }


    [ServerRpc]
    private void spawnServerRpc(){
        GameObject item = Instantiate(spawn, this.transform.position + this.transform.forward, spawn.transform.rotation);
        item.GetComponent<NetworkObject>().Spawn(true);
    }
}
