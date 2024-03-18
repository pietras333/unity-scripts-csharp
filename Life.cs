using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    [Header("Life")]
    [Space] 
    [Header("Neccessary")]
    [Space]
    [SerializeField] Move move;
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] bool player;

    [Space]
    [Header("Customizable")]
    [Space]
    [Header("Health")]
    [SerializeField, Range(0,100)] float healthMax;
    [HideInInspector] float currentHealth;
    [Header("Stamina")]
    [SerializeField, Range(0,100)] float staminaMax;
    [SerializeField, Range(0,20)] float staminaIncrease;
    [SerializeField, Range(0,20)] float staminaDecrease;
    [HideInInspector] public float currentStamina;

    [Header("HUD")]
    [SerializeField] Slider slider;
    

    void Awake(){
        currentHealth = healthMax;
        currentStamina = staminaMax;
    }

    void Update(){
        if(currentHealth <= 0f){
            this.transform.gameObject.SetActive(false);
            Invoke("heal", 1f);
        }

        if(player){

            currentStamina = Mathf.Clamp(currentStamina, 0f, staminaMax);
            slider.value = Mathf.Lerp(slider.value, currentStamina * 0.01f, 10f * Time.deltaTime);
            if(move.sliding){
                decreaseStamina(0.75f);
            }
            if(!move.sliding && !weaponManager.aiming){
                increaseStamina();
            }
        }
        
        currentHealth = Mathf.Clamp(currentHealth, 0f , healthMax);
    }

    public void getDamage(float damage){
        currentHealth -= damage;
        Debug.Log("Current Health: " + currentHealth);
    }

    void heal(){
        currentHealth = healthMax;
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(0,0,0), 5 * Time.deltaTime);
    }

    public void decreaseStamina(float multiplier){
        currentStamina -= staminaDecrease * multiplier * Time.deltaTime;
    }
    public void increaseStamina(){
        if(currentStamina < staminaMax){
            currentStamina += staminaIncrease  * Time.deltaTime;
        }
    }
    
}
