using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverFX;
    public AudioClip clickFX;

    public void ButtonHover(){
        audioSource.PlayOneShot(hoverFX);
    }

    public void ButtonClick(){
        audioSource.PlayOneShot(clickFX);
    }
}
