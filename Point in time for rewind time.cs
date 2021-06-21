using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;

    public PointInTime (Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}
