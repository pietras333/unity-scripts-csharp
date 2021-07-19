using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTeleport : MonoBehaviour
{
    // with simple grab/drop system u can use this for ender pearl like effect just after throw set doTeleport to true 
    public GameObject player;
    [HideInInspector] public bool doTeleport = false;
    private GameObject thisGameObject;
    private Vector3 teleportPosition;

    public void Start(){
        thisGameObject = this.gameObject;
    }
    public void OnCollisionEnter(Collision collider){
      if(doTeleport){
        teleportPosition = new Vector3(thisGameObject.transform.position.x, thisGameObject.transform.position.y, thisGameObject.transform.position.z);
        player.transform.position = teleportPosition;
        Destroy(thisGameObject);
      }
    }
}
