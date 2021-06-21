using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTime : MonoBehaviour
{
    public bool isRewind = false;
    public Rigidbody rb;
    List<PointInTime> pointsInTime;
    
    private void Start()
    {
        pointsInTime = new List<PointInTime>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rewind"))
        {
            StartRewind();
            Invoke("StopRewind", 3f); 
        }
    }

    private void FixedUpdate()
    {
        if (isRewind)
            Rewind();
        else
            Record();
    }

    void Rewind()
    {
        if(pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;
            pointsInTime.RemoveAt(0);   
        }
        else
        {
            StopRewind();
        }
    }

    void Record()
    {
        pointsInTime.Insert(0, new PointInTime(transform.position,transform.rotation));
    }

    void StartRewind()
    {
        isRewind = true;
        rb.isKinematic = true;
    }

    void StopRewind()
    {
        isRewind = false;
        rb.isKinematic = false;
    }
}
