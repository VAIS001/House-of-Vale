using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    private GameObject lastHit;
    public  bool canCombo;
    private const float comboWindow = 1f;
    private Coroutine comboCoroutine;
    private int comboCount=0;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag(this.tag)) //different tags
        {
            //calculate damage
            CharacterStats wielderStats = transform.parent.GetComponent<CharacterStats>();
            if(wielderStats==null) return; //no way a weapon swinging itself
            
            //character stats: str, dex, weapon stats: damage
            float damage = 1f; //calculate

            if(canCombo){
                if(other.gameObject == lastHit){
                    damage+=comboCount;
                } else{
                    comboCount = 0; //reset
                }
            }
            
             bool attackLanded = other.gameObject.GetComponent<WeaponController>().TakeHit(this.gameObject, damage, transform.parent.GetComponent<WeaponController>().attackIsWeighty);
             if (attackLanded)
             {
                if(comboCoroutine != null)StopCoroutine(comboCoroutine);
                comboCoroutine = StartCoroutine(ComboCoroutine());
             } else{ 
                ResetCombo();
             }
        }
    }

    private void ResetCombo () {
        if(comboCoroutine!=null)StopCoroutine(comboCoroutine);
        comboCount=0;
        lastHit=null;
        canCombo=false;
    }

    private IEnumerator ComboCoroutine(){
        canCombo = true;
        comboCount++;
        yield return new WaitForSeconds(comboWindow);
        canCombo = false;
    }

}
