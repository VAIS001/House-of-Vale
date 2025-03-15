using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach to any object capable of being a weapon
//weapon should be a child of wielder
//attackbox should be a child of weapon
public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject attackbox;
    public bool attackIsWeighty=false;
    private GameObject wielder;
    public WeaponStats weaponStats;//{get; private set;}
    private bool isBlocking;
    private float blockDuration = 0f;
    private const float parryWindow = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        wielder = transform.parent.gameObject;
        GameEvents.GetInstance().OnBlockChanged += BlockChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBlocking){
            //reduce poise
            blockDuration += Time.deltaTime;
        } else {
             blockDuration = 0f;
            //recover poise
        }
        if(wielder.GetComponent<CharController>().charStats.poise <= 0){
            //long_stun character controller
        }
    }

    private void OnDestroy() {
        GameEvents.GetInstance().OnBlockChanged -= BlockChanged;
    }

    //call from character controller when player interacts
    public void PickUp(){
        //change wielder
    }

    public bool TakeHit(GameObject attacker, float damage, bool heavyHit){
        if(isBlocking && !heavyHit) {
            if(blockDuration<parryWindow){
                Parry(attacker);
                return false; //attack blocked
            }
            //take poise damage
            //block attack
            return false; //attack blocked
        } else{
            //take damage
            wielder.GetComponent<CharController>().charStats.health -= damage;
            if (heavyHit)
            {
                //apply physics force
            }

            return true; //attack landed
        }
    }

    
    private void BlockChanged(bool blocking){
       
        isBlocking = blocking;
    }

    private void Parry(GameObject attacker){
        //brief_stun attacker character controller
    }

    //call from animator during anticipation phase of animation 
    public void Anticipation (){
        //do cool stuff like slow mo for weighty attack
        //or in-combat speech/dialogue
    }

    //call from animator during strike phase of animation 
    public void Strike(){
        if (attackbox==null)return;
        attackbox.SetActive(true);

        //undo slow mo or fast mo
    }

    //for cool stuff like throwing axe, shooting arrows
    public void Throw(){
        //add force
        //start coroutine
        //attack hitbox check for hits
        //character stat dex for accuracy
        //boomerang back if hit
    }

    
}
