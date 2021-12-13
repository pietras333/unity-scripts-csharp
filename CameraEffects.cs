using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.Rendering.PostProcessing;
public class CameraEffects : MonoBehaviour
{
    [Header("Camera Effects")]
    [Header("Core")]
    [SerializeField] PostProcessVolume volume;
    [SerializeField] PlayerGun playerGunRef;
    [SerializeField] PlayerMovement playerMovementRef;
    [SerializeField] float actualSpeed;
    [Header("Particle systems")]
    [SerializeField] ParticleSystem speedTrails;
    [Header("Post Processing")]
    [HideInInspector] DepthOfField depthOfField;
    [HideInInspector] Vignette vignette;
    public void Update()
    {
        actualSpeed = playerMovementRef.actualSpeed;
        CameraShake();
        SpeedTrailSystem();
        AdjustDepthOfField();
        AdjustVignette();
    }
    public void CameraShake()
    {
        if (actualSpeed > 4f && actualSpeed < 12.9f)
            CameraShaker.Instance.ShakeOnce(.25f, .25f, .1f, 1f);
        if (actualSpeed > 13f)
            CameraShaker.Instance.ShakeOnce(.4f, .4f, .1f, 1f);
    }
    public void SpeedTrailSystem()
    {
        float emissionOverSpeed = actualSpeed * 3.5f;
        float speedOverSpeed = actualSpeed / 2.5f;
        speedTrails.startSpeed = speedOverSpeed;
        var emission = speedTrails.emission;
        emissionOverSpeed = Mathf.Clamp(emissionOverSpeed, 0f, 30f);
        emission.rateOverTime = emissionOverSpeed;
    }
    public void AdjustDepthOfField()
    {
        volume.profile.TryGetSettings(out depthOfField);
        if (playerGunRef.isScoping)
            depthOfField.aperture.value = Mathf.Lerp(depthOfField.aperture.value, 32f, 10f * Time.deltaTime);
        else
            depthOfField.aperture.value = Mathf.Lerp(depthOfField.aperture.value, 10f, 10f * Time.deltaTime);
    }
    public void AdjustVignette()
    {
        volume.profile.TryGetSettings(out vignette);
        if (playerMovementRef.isGrounded)
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, .65f, 1f * Time.deltaTime);
        else if (!playerMovementRef.isGrounded)
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, .01f, 1f * Time.deltaTime);
    }
}
