using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    /// min footsteps pitch 0.65
    /// max footsteps pitch 1
    [Header("Sound Manager")]
    [Header("Core")]
    [SerializeField] float actualSpeed;
    [SerializeField] PlayerMovement playerMovementRef;
    [Header("Footsteps")]
    [SerializeField] AudioSource footstepsAudioSource;
    [SerializeField] float minFootstepsPitch;
    [SerializeField] float maxFootstepsPitch;

    public void Update()
    {
        GatherData();
        if (!playerMovementRef.isSliding && actualSpeed > 2f && playerMovementRef.isGrounded)
            SetFootstepsPitch();
        else
            footstepsAudioSource.enabled = false;
    }
    public void GatherData()
    {
        actualSpeed = playerMovementRef.actualSpeed;
    }
    public void SetFootstepsPitch()
    {
        footstepsAudioSource.enabled = true;
        float footstepsPitch;
        footstepsPitch = actualSpeed / 10;
        footstepsPitch = Mathf.Clamp(footstepsPitch, minFootstepsPitch, maxFootstepsPitch);
        footstepsAudioSource.pitch = footstepsPitch;
    }
}
