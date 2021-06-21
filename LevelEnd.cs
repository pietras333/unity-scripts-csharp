using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class GameEnd : MonoBehaviour
{
    public Player_Movement playermovement;
    public Rigidbody rb;
    public TextMeshProUGUI timer;
    float currentTime;
    bool isActive = false;
    public bool didEnd = false;

    private void Start()
    {
        currentTime = 0;
        isActive = true;
        didEnd = false;

    }

    void Update()
    {

        if (isActive)
            currentTime = currentTime + Time.deltaTime; 
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timer.text = time.ToString(@"mm\:ss\:ff");


    }

    void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex + 1 < 4)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
        Debug.Log("u've completed all maps");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Finish"))
        {
            didEnd = true;
            isActive = false;
            string finishTime = currentTime.ToString();
            timer.text = finishTime;  
            playermovement.enabled = false;
            Debug.Log("YOU'VE FINISHED THE MAP");  
            Invoke("LoadNextLevel", 5f);
        }
    }
}
