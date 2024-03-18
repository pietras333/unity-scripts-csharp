using UnityEngine;
using UnityEngine.UI;

public class HitMarkHandler : MonoBehaviour
{
    [Header("Hit Mark Handler")]
    [Space]
    [Header("Neccesary")]
    [Space]
    [SerializeField] Color baseColor;
    [HideInInspector] Image hitMark;
    
    [Space]
    [Header("Customizable")]
    [Space]
    [SerializeField, Range(0,1)] float longevity;
    [SerializeField] Color headshotColor;
    [SerializeField] Color torsoColor;
    [SerializeField] Color limbColor;
    
    
    void Start(){
        hitMark = this.GetComponent<Image>();
        hitMark.enabled = false;
    }

    public void headHit(){
        hitMark.enabled = true;
        hitMark.color = headshotColor;
        Invoke("reset", longevity);
    }
    public void torsoHit(){
        hitMark.enabled = true;
        hitMark.color = torsoColor;
        Invoke("reset", longevity);
    }
    public void limbHit(){
        hitMark.enabled = true;
        hitMark.color = limbColor;
        Invoke("reset", longevity);
    }

    public void reset(){
        hitMark.enabled = false;
        hitMark.color = baseColor;
    }
    
}
