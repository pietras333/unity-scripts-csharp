using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public enum ObjectiveType
    {
        Kill,
        Steal
    }

    public enum ObjectiveMode
    {
        // Main = group
        // Secondary = individual
        Main,
        Secondary
    }

    [Header("Objective")]
    [Space]
    [Header("Objective representation")]
    [Space]
    [SerializeField]
    public string objectiveDescription;

    [Header("Configuration")]
    [SerializeField]
    public ObjectiveType objectiveType;

    [SerializeField]
    public ObjectiveMode objectiveMode;

    [SerializeField]
    public bool isCompleted = false;

    [SerializeField]
    public GameObject target;

    [SerializeField]
    public bool drawGizmos = true;

    public delegate void ObjectiveCompleted(Objective objective);

    public static event ObjectiveCompleted OnObjectiveCompleted;

    public void CompleteObjective()
    {
        if (isCompleted)
            return;

        isCompleted = true;

        OnObjectiveCompleted?.Invoke(this);

        Debug.Log("Objective completed");
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = isCompleted ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 1.5f);
    }
}
