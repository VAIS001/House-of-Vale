using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;

namespace TL.UtilityAI
{
    public class Action : ScriptableObject
    {
        public string Name;
        private float _score;
        public delegate void ActionToExecute();
        public ActionToExecute actionToExecute;
        public float score
        {
            get { return _score; }
            set
            {
                this._score = Mathf.Clamp01(value);
            }
        }

        public Consideration[] considerations;

        public Transform RequiredDestination { get; protected set; }

        public virtual void Awake()
        {
            score = 0;
        }

        //public abstract void Execute();
        public void Execute()
        {
            //play animation
            if(actionToExecute != null) {
                actionToExecute();
            } 

        }


        //public virtual void SetRequiredDestination(NPCController npc) { }
    }
}
