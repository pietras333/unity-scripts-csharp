using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void VolumeSlide(float volume){
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }
}
