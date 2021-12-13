using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    /// move smoothness 5
    [Header("Player Gun")]
    [Header("Core")]
    [SerializeField] Transform playerGun;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform mainCamera;
    [SerializeField] Transform gunTip;
    [SerializeField] public bool isReloading;
    [Header("Customization")]
    [SerializeField] public KeyCode fireKey;
    [SerializeField] KeyCode reloadKey;
    [SerializeField] public float actualAmmo;
    [SerializeField] float maxAmmo;
    [Header("UI Display")]
    [SerializeField] TextMeshProUGUI actAmmUI;
    [SerializeField] TextMeshProUGUI maxAmmUI;
    [Header("Bullet Prediction")]
    [SerializeField] Material spherePointerMaterial;
    [SerializeField] Transform spherePointer;
    [SerializeField] float maxPredicitonDistance;
    [SerializeField] RaycastHit hit;
    [SerializeField] LayerMask spherePointerLayer;
    [Header("Gun Tip Trail")]
    [SerializeField] GameObject trailRenderer;
    [Header("Animation")]
    [SerializeField] Animator gunAnimator;
    [Header("Scope")]
    [SerializeField] public bool isScoping;
    [SerializeField] KeyCode scopeKey;
    [SerializeField] Vector3 normalPos;
    [SerializeField] Vector3 scopePos;
    [SerializeField] float moveSmoothness;
    [Header("Particle System")]
    [SerializeField] ParticleSystem gunFlashParticleSystem;
    public void Update()
    {
        actAmmUI.text = actualAmmo.ToString();
        maxAmmUI.text = maxAmmo.ToString();
        if (Input.GetKeyDown(fireKey) && (actualAmmo <= maxAmmo && actualAmmo > 0) && !isReloading)
            Shoot();
        if (actualAmmo < maxAmmo && Input.GetKeyDown(reloadKey) && !isReloading)
            Reload(actualAmmo);

        if (!isReloading && Input.GetKey(scopeKey))
            StartScope();
        else if (isReloading || Input.GetKeyUp(scopeKey))
            StopScope();
        PredictBulletEndPosition();
    }
    public void Shoot()
    {
        spherePointer.gameObject.SetActive(true);
        gunAnimator.SetBool("isShooting", true);
        Invoke("SetTrailToFalse", .5f);
        Invoke("SetisShootingToFalse", .2f);
        actualAmmo -= 1;
        trailRenderer.SetActive(true);
        Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
        gunFlashParticleSystem.Play();
    }
    public void Reload(float actAmm)
    {
        spherePointer.gameObject.SetActive(false);
        gunAnimator.SetBool("isReloading", true);
        Invoke("isReloadingFalse", 1.5f);
        trailRenderer.SetActive(true);
        isReloading = true;
        float ammoToAdd = maxAmmo - actualAmmo;
        actualAmmo += ammoToAdd;
    }
    public void isReloadingFalse()
    {
        trailRenderer.SetActive(false);
        gunAnimator.SetBool("isReloading", false);
        isReloading = false;
    }
    public void PredictBulletEndPosition()
    {
        if (Physics.Raycast(gunTip.position, gunTip.forward, out hit, maxPredicitonDistance))
        {
            spherePointer.gameObject.SetActive(true);
            if (hit.transform.gameObject.layer != spherePointerLayer)
                spherePointer.transform.position = hit.point;
            SetPointersMaterialAlpha(hit.distance);
        }
        else
        {
            spherePointer.gameObject.SetActive(false);
        }
    }
    public void SetisShootingToFalse()
    {
        gunAnimator.SetBool("isShooting", false);
    }
    public void SetTrailToFalse()
    {
        trailRenderer.SetActive(false);
    }
    public void SetPointersMaterialAlpha(float distance)
    {
        var color = new Color();
        color.a = Mathf.Clamp(color.a, 40f, 80f);
        color.b = 1f;
        color.r = 1f;
        color.g = 1f;
        color.a = distance;
        spherePointerMaterial.color = color;
    }
    public void StartScope()
    {
        isScoping = true;
        playerGun.transform.localPosition = Vector3.Lerp(playerGun.transform.localPosition, scopePos, moveSmoothness * Time.deltaTime);
    }
    public void StopScope()
    {
        isScoping = false;
        playerGun.transform.localPosition = Vector3.Lerp(playerGun.transform.localPosition, normalPos, moveSmoothness * Time.deltaTime);
    }
}
