using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   public GameObject MainMenu;
   public GameObject OptionsMenu;
   public TextMeshProUGUI buttonBackText;
   bool isInOptions = false;

   public void Play(){
      SceneManager.LoadScene("Lobby");
   }

   public void Options(){
      isInOptions = true;
   }

   public void QuitOrBack(){
      if(isInOptions){
         isInOptions = false;
         MainMenu.SetActive(true);
         OptionsMenu.SetActive(false);
      }else if(!isInOptions){
         Application.Quit();
      }
   }

   public void Update(){
      if(isInOptions){
         MainMenu.SetActive(false);
         OptionsMenu.SetActive(true);
         buttonBackText.text = "back";
      }else if(!isInOptions){
         buttonBackText.text = "quit";
      }
   }
}
