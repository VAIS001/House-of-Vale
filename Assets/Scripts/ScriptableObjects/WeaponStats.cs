using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Vabalands/WeaponStats", order = 0)]
public class WeaponStats : ScriptableObject {
    public string weaponName="";
    public bool isBow = false; //have different animation
    public bool isFist = false;
    public float weight = 3f;
    public string description;
    public float damage;
    public float defense;
    public float cost;
    public float lifespan;
    public WeaponSkill weaponSkill;
    public Rarity rarity; //likellihood of weapon to drop from loot

    public enum WeaponSkill{

    }

    public enum Rarity{

    }
}
