using System;
using UnityEngine;
using TarodevController;

public class CombatController : MonoBehaviour {

    //make sure to attach fist weapon to character to enable sheathing of sword
    public WeaponController[] weapons;
    private WeaponController activeWeapon;
    private PlayerAnimator playerAnimator;
    private int activeWeaponIndex=0;

    public Transform handBone;
    public Transform torsoBone;


    private bool weaponEquipped = false;
    private const float deadZoneY = 0.2f;

    private void Start() {
        playerAnimator = GetComponent<PlayerAnimator>();
        weapons = GetComponentsInChildren<WeaponController>();
        Debug.Log("Weapons found : "+weapons.Length);
    }
    
    public void Attack(bool heavyAttack = false, float attackY = 0f){
        
        if(!weaponEquipped){
            EquipWeapon();
            return;
        }

        if(activeWeapon.weaponStats.isBow){
            // bow animations are different
            if(heavyAttack) {
                
            } else {
                
            }
            return;
        }

        if(activeWeapon.weaponStats.isFist) {
            heavyAttack = false; //unless you have a whole set of other animations for it
        }

        //play animation
        if(heavyAttack){
           if(attackY > deadZoneY) {
                playerAnimator.PlayAnimation(AnimationType.HeavyAttackDown);
           } else if(attackY < -deadZoneY){
                playerAnimator.PlayAnimation(AnimationType.HeavyAttackUp);
           } else{
                playerAnimator.PlayAnimation(AnimationType.HeavyAttackMid);
           }
        } else{
            if(attackY > deadZoneY) {
                playerAnimator.PlayAnimation(AnimationType.AttackDown);
           } else if(attackY < -deadZoneY){
                playerAnimator.PlayAnimation(AnimationType.AttackUp);
           } else{
                playerAnimator.PlayAnimation(AnimationType.AttackMid);
           }
        }
        //animation event will trigger attack
    }

    private void EquipWeapon(){
        playerAnimator.PlayAnimation(AnimationType.SwitchWeapon);
    }

    private void UnEquipWeapon(bool switchingWeapon){
        if(switchingWeapon){
            //animation same as unequip weapon but animation event calls SwitchWeapons method
            playerAnimator.PlayAnimation(AnimationType.SwitchWeapon);
        }
        playerAnimator.PlayAnimation(AnimationType.SwitchWeapon);
    }

    //call from animator event or when picking weapon
    public void WeaponToHand(){
        activeWeapon.gameObject.transform.SetParent(handBone);
        //GameEvents.GetInstance().WeaponEquipped(activeWeapon.weaponStats.weight); //TODO Debug
        weaponEquipped = true;
    }

    //call from animator event when sheathing weapon
    public void WeaponToTorso(){
        if(activeWeapon == null)return; //has no weapon on
        activeWeapon.gameObject.transform.SetParent(torsoBone);
        //GameEvents.GetInstance().WeaponUnEquipped(activeWeapon.weaponStats.weight); //TODO Debug
        weaponEquipped = false;
    }

    public void Block(float blockY = 0f){
        //play animation
        if (blockY > deadZoneY)
        {
            playerAnimator.PlayAnimation(AnimationType.BlockUp);
        }else if(blockY < -deadZoneY){
            playerAnimator.PlayAnimation(AnimationType.BlockDown);
        } else{
            playerAnimator.PlayAnimation(AnimationType.BlockMid);
        }
    }

    //called from animator
    private void SwitchWeapons(){
        if(weapons.Length == 0)return;
        WeaponToTorso();
        activeWeaponIndex++;
        if(activeWeaponIndex >= weapons.Length)activeWeaponIndex=0;
        activeWeapon = weapons[activeWeaponIndex];
        WeaponToHand();
    }


}