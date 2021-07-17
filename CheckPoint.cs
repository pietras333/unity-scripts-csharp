using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    // here u need to create a tag and call it CheckPoint and apply it on any game object that u will use as checkpoint
    [SerializeField] GameObject camera;
    private GameObject player;
    private GameObject collidedGO;
    private Vector3 checkPointPosition;
    private bool haveCheckPoint = false;

    private void Awake(){
        player = this.gameObject;
    }

    void Update(){
        RaycastHit hit;
        if(Input.GetKeyDown(KeyCode.E)){
            if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2f)){
                GameObject checkPointObject = hit.transform.gameObject;
                if(checkPointObject.CompareTag("CheckPoint")){
                    haveCheckPoint = true;
                    checkPointPosition = new Vector3(checkPointObject.transform.localPosition.x, checkPointObject.transform.localPosition.y + 2f, checkPointObject.transform.localPosition.z);
                }
            }
        }

        if(haveCheckPoint && Input.GetKeyDown(KeyCode.R)){
            player.transform.position = checkPointPosition;
        }else if(Input.GetKeyDown(KeyCode.R) && !haveCheckPoint){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
