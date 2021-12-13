using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeFunctions : MonoBehaviour
{
    [Header("Player Life Functions")]
    [Header("Health")]
    [HideInInspector] public int actualHealth;
    [HideInInspector] PlayerMovement playerMovement;
    [HideInInspector] GameObject playerModel;
    public void Start()
    {
        GatherOrSetData();

    }
    public void Update()
    {
        AnalysePlayerHealth();
    }
    public void AnalysePlayerHealth()
    {
        if (actualHealth <= 0f)
            KillPlayer();
    }
    public void KillPlayer()
    {
        playerModel.SetActive(false);
        playerMovement.enabled = false;
    }
    public void GatherOrSetData()
    {
        actualHealth = 100;
        playerModel = this.gameObject.transform.GetChild(0).gameObject;
        playerMovement = this.gameObject.GetComponent<PlayerMovement>();
    }
}
