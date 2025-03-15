using System;
using UnityEngine;

public class GameStats : MonoBehaviour {

    private static GameStats instance;
    public float speed=0f;
    public bool isBlocking=false;

    private void Awake(){
        if(instance!=null && instance !=this){
            Destroy(instance);
        }
        if(instance == null)instance = this;
    }

    public static GameStats GetInstance(){
        return instance;
    }

}