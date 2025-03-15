using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Vabalands/CharacterStats", order = 0)]
public class CharacterStats : ScriptableObject {
    public float str=2f;
    public float dex=2f;
    public float intl=2f;
    public float health=2f;
    public float stamina=2f;
    public float poise = 2f;
    public float cha=2f;
    public float wlp=2f;
    public float lck=2f;
    public float agi=2f;
    public float per=2f; 
    public float morality=2f;
    public float player_amity=2f;
    public Trait trait;
    public Faction faction;

    public enum Trait
    {
        jovial,
        loyal,
        rival,
        support,
        temper,
        chatty,
        cunny, // good in negotiations, 
        proud //disobeys orders, 

    }

    public enum Faction
    {
        Vale,
        Oceanlands,
    }
}

