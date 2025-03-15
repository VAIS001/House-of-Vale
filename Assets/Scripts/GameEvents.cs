using System;
using UnityEngine;

public class GameEvents : MonoBehaviour {

    private static GameEvents _instance;

    public event Action<float> OnWeaponEquipped;
    public event Action<float> OnWeaponUnEquipped;
    public event Action<bool> OnBlockChanged;
    public event Action<float> OnHealthChanged;
    public event Action<float> OnStaminaChanged;
    

    #region Narration 

    //store player encounters
    // kills, combatduration, runspeed, jumps, jumptricks, stealthruns, retreats 
    
    #endregion

    private void Awake() {
        if(_instance!=null && _instance!=this){
            Destroy(_instance);
        } else {
            _instance = this;
        }
    }

    public static GameEvents GetInstance(){
        //if(_instance == null) {
          //  _instance = this;
        //} 
        return _instance;
    }

    private void Start() {
        
    }

    public void WeaponEquipped(float drag){
        if(OnWeaponEquipped!= null){
            OnWeaponEquipped(drag);
        }
    }

    public void WeaponUnEquipped(float drag){
        if(OnWeaponUnEquipped!= null){
            OnWeaponUnEquipped(drag);
        }
    }

    public void BlockChanged (bool blocking) {
        if(OnBlockChanged!=null) {
            OnBlockChanged(blocking);
        }
    }

    public void HealthChanged(float newHealth){
        if(OnHealthChanged !=null) {
            OnHealthChanged(newHealth);
        }
    }

    public void StaminaChanged(float newStamina){
        if(OnStaminaChanged !=null) {
            OnStaminaChanged(newStamina);
        }
    }

}