using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScaleIn : MonoBehaviour
{
    #region Inspector Fields

    [Header("Line Scale In")]
    [Space]
    [Header("Configuration")]
    [Tooltip("The original scale to which the line will scale up.")]
    [SerializeField]
    private Vector3 originalScale = new Vector3(0.1f, 0.5f, 0.1f);

    [Tooltip("The smoothness factor for scaling.")]
    [SerializeField]
    public float smoothness = 1f;

    [Tooltip("Determines if the line should scale up.")]
    [SerializeField]
    private bool scaleUp = false;

    #endregion

    private void Start()
    {
        // Initialize the line scale to zero at the start
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        // Return early if scaling is not enabled
        if (!scaleUp)
        {
            return;
        }

        // Stop scaling once the original scale is reached
        if (transform.localScale == originalScale)
        {
            scaleUp = false;
            return;
        }

        // Smoothly scale the line towards the original scale
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale,
            Time.deltaTime * smoothness * 2f
        );
    }

    // Public method to start the scaling process
    public void StartScalingIn()
    {
        scaleUp = true;
    }
}
