using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealObjective : Objective
{
    [Header("Steal Objective")]
    [Space]
    [Header("References")]
    [SerializeField]
    GameObject extractionPoint;

    [HideInInspector]
    IsInExtraction isInExtraction;

    private void Start()
    {
        objectiveType = Objective.ObjectiveType.Steal;
        isInExtraction = extractionPoint.AddComponent<IsInExtraction>();
        isInExtraction.targetObject = target;
        isInExtraction.objective = this;
    }
}
