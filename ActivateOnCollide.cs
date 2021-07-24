using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnCollide : MonoBehaviour
{
  [SerializeField] GameObject objectToActiavate;
  public void OnCollisionEnter(Collision collider){
      if(collider.gameObject.CompareTag("Grabable")){
          objectToActiavate.SetActive(true);
      }
  }
}
