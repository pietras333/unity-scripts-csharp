using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTeleport : MonoBehaviour
{
    public GameObject player;
    [HideInInspector] public bool doTeleport = false;
    [SerializeField] int minYlevel;
    private Vector3 objectPosition;
    private GameObject thisGameObject;
    private Vector3 teleportPosition;
    
    public void Start(){
      thisGameObject = this.gameObject;
      objectPosition = new Vector3(thisGameObject.transform.position.x, thisGameObject.transform.position.y, thisGameObject.transform.position.z);
    }
    public void Update(){
      if(thisGameObject.transform.position.y < minYlevel && minYlevel != 0){
        doTeleport = false;
        thisGameObject.transform.position = objectPosition;
      }
    }
    public void OnCollisionEnter(Collision collider){
      if(doTeleport){
        teleportPosition = new Vector3(thisGameObject.transform.position.x, thisGameObject.transform.position.y, thisGameObject.transform.position.z);
        player.transform.position = teleportPosition;
        Destroy(thisGameObject);
      }
    }
}
