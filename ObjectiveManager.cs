using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Objective Manager")]
    [Header("References")]
    private Dictionary<Objective.ObjectiveType, Objective> objectiveDictionary =
        new Dictionary<Objective.ObjectiveType, Objective>();

    // [SerializeField]
    // public bool allObjectivesCompleted = false;

    [SerializeField]
    public bool mainObjectivesCompleted = false;

    [SerializeField]
    public bool sideObjectivesCompleted = false;
    public delegate void MainObjectivesCompletedHandler();
    public event MainObjectivesCompletedHandler OnMainObjectivesCompleted;
    public delegate void SideObjectivesCompletedHandler();
    public event SideObjectivesCompletedHandler OnSideObjectivesCompleted;

    [SerializeField]
    List<Objective> mainObjectives = new List<Objective>();

    [SerializeField]
    List<Objective> sideObjectives = new List<Objective>();

    public void Start()
    {
        Objective[] objectives = FindObjectsOfType<Objective>();
        foreach (Objective objective in objectives)
        {
            objectiveDictionary.Add(objective.objectiveType, objective);
        }
        Objective.OnObjectiveCompleted += HandleObjectiveCompleted;

        mainObjectives = GetSpecificObjectives(Objective.ObjectiveMode.Main);
        sideObjectives = GetSpecificObjectives(Objective.ObjectiveMode.Secondary);
    }

    public List<Objective> GetSpecificObjectives(Objective.ObjectiveMode mode)
    {
        return objectiveDictionary
            .Where(obj =>
                mode == Objective.ObjectiveMode.Main
                    ? obj.Value.objectiveMode == Objective.ObjectiveMode.Main
                    : obj.Value.objectiveMode == Objective.ObjectiveMode.Secondary
            )
            .Select(obj => obj.Value)
            .ToList();
    }

    private void HandleObjectiveCompleted(Objective objective)
    {
        Debug.Log("Objective Complete: " + objective.objectiveType.ToString());
        mainObjectivesCompleted = CheckSpecificObjectivesCompleted(Objective.ObjectiveMode.Main);
        sideObjectivesCompleted = CheckSpecificObjectivesCompleted(
            Objective.ObjectiveMode.Secondary
        );
    }

    private bool CheckSpecificObjectivesCompleted(Objective.ObjectiveMode mode)
    {
        foreach (Objective objective in GetSpecificObjectives(mode))
        {
            if (!objective.isCompleted)
            {
                return false;
            }
        }

        if (mode == Objective.ObjectiveMode.Main)
            TriggerMainObjectivesCompleted();
        else
            TriggerSideObjectivesCompleted();
        return true;
    }

    private void TriggerMainObjectivesCompleted() => OnMainObjectivesCompleted?.Invoke();

    private void TriggerSideObjectivesCompleted() => OnSideObjectivesCompleted?.Invoke();
}
