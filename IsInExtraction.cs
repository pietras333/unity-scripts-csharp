using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInExtraction : MonoBehaviour
{
    [Header("Is In Extraction")]
    [Space]
    [Header("References")]
    [SerializeField]
    public GameObject targetObject;

    [SerializeField]
    public Objective objective;

    [HideInInspector]
    BoxCollider boxCollider;

    [HideInInspector]
    bool isCompleted = false;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (targetObject == collision.gameObject)
        {
            Debug.Log("In extraction");
            objective.CompleteObjective();
            isCompleted = true;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = isCompleted ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }
}
