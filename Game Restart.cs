using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public GameEnd gameEndRef;
    private void Update()
    {
        if (Input.GetKeyDown("r"))
            Restart();
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!gameEndRef.didEnd){
             if (collision.gameObject.CompareTag("RestartGame"))
                 Restart();
        }
    }
}
