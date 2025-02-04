using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppModule : MonoBehaviour
{
    [Header("App Module")]
    [Space]
    [Header("Configuration")]
    [SerializeField]
    public string appName = "appName";

    [SerializeField]
    public bool isInSocket;

    [SerializeField]
    Vector3 socketPosition;

    public void Update()
    {
        if (isInSocket)
        {
            transform.SetPositionAndRotation(socketPosition, Quaternion.identity);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("App Module entered socket.");
        if (other.gameObject.CompareTag("App Module Socket"))
        {
            Debug.Log("App Module entered socket.");
            this.GetComponent<Rigidbody>().isKinematic = true;
            isInSocket = true;
            socketPosition = other.transform.position;
            other.GetComponent<AppModuleReader>().currentAppModule = this;
        }
    }
}
