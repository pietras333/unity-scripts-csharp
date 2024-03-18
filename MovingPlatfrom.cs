using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Moving Platfrom")]
    [Space]
    [Header("Customizable")]
    [Space]
    [SerializeField, Range(0,10)] float speed;
    [SerializeField] Vector3[] directions; 
    [HideInInspector] Vector3 originalPosition;

    void Start(){
        originalPosition = transform.position;
        StartCoroutine(MoveThroughDirections());
    }
    
    IEnumerator MoveThroughDirections()
    {
        while(true){
            foreach (Vector3 direction in directions){
                Vector3 targetPosition = originalPosition + direction;

            
                while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                    yield return null;
                }

                while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }

    void OnCollisionStay(Collision collider){
        collider.transform.GetComponent<Rigidbody>().velocity += this.GetComponent<Rigidbody>().velocity;
    }
}
