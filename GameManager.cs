using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Manager")]
    [Space]
    [Header("References")]
    [SerializeField]
    private ObjectiveManager objectiveManager;

    private void Start()
    {
        if (objectiveManager)
        {
            // Subscribe event
            objectiveManager.OnMainObjectivesCompleted += HandleMainObjectivesCompleted;
            objectiveManager.OnSideObjectivesCompleted += HandleSideObjectivesCompleted;
        }
        objectiveManager.Start();
    }

    private void OnDestroy()
    {
        if (objectiveManager)
        {
            // Unsubscribe from event
            objectiveManager.OnMainObjectivesCompleted -= HandleMainObjectivesCompleted;
            objectiveManager.OnSideObjectivesCompleted -= HandleSideObjectivesCompleted;
        }
    }

    private void HandleMainObjectivesCompleted()
    {
        Debug.Log("!!Main Objectives Complete, GAME OVER!!");
    }

    private void HandleSideObjectivesCompleted()
    {
        Debug.Log("!!Side Objectives Complete!!");
    }
}
