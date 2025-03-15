using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;

//npcs can talk and fight
//unlike enemies, they only fight back when attacked.
public class NPCController : MonoBehaviour {

    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        
    }

    //combat actions
    //create different enemy types by combining different considerations, and unlocking different moves
    /*
        considerations:
        previous action success - offensive, aggressive type
        previous action failure - defensive, cautious type
        novel? not been used recently
        player's current action: currently blocking? currently attacking? currently
        anticipation: player's past actions
        instincts: attack sequence, defense sequence, random sequence - rogues, bandits 
        style: attacking, defending
        health, stamina, vabarim
        player's health, stamina, vabarim, number of engagements
        for movement - closeness of traps
    */

    /*  
        basic attack
        chained attack
        heavy attack
        block
        move - jump, roll, dash
        switch weapon
        push, throw
        retreat
        dialogue attack: convert player ally, reduce player's vabarim
    */
    #region CombatMoves

    public void BasicAttack () {
        
    }

    public void HeavyAttack () {
        
    }

    public void Block(){

    }

    public void Throw(){

    }

    public void DialogueAttack(){

    }

    public void DialogueDefend(){

    }

    public void DialogueObject(){

    }

    public void RandomAttackSequence(){

    }

    public void SwitchWeapons () {
        
    }

    public void Move () {
        
    }



    #endregion
    //dialogue 
    /*
        actions:
            greet
            give history
            give clue
            talk relationship
            discuss current affairs
            attack: accuse
            combo
            heavy attack: object 
            block: defend
            parry: stun           

    */
    #region Dialogue
    // dialogue options for non scripted parts of the game
    // use utility AI to pick for NPC

    
    public void Acknowledge () {
        //takes into account, player's current stats and action counts    
    }

    public void Accuse () {
        //takes into account, player's recent kills, robberies
    }

    public void ShareCurrentWorldInfo () {

    }

    public void StoryGuide () {

    }

    public void MapGuide () {
        
    }

    public void Jokes () {

    }
    
    public void AttractionLines (){

    }

    public void MentorTips () {
        
    }

    public void TalkRelationship () {

    }

    public void ShareHistory () {
        
    }
    #endregion


}