using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

namespace TL.UtilityAI
{
    [CreateAssetMenu(fileName = "Consideration", menuName = "Vabalands/Utility AI/Consideration")]
    public class Consideration : ScriptableObject
    {
        public string Name;
        

        private float _score;
        public float attributeMax;
        public float attributeInitialValue;
        [SerializeField]private float attributeValue;
        public AttributeType attributeType;
        [SerializeField] private AnimationCurve responseCurve;

        public float score
        {
            get { return _score; }
            set
            {
                this._score = Mathf.Clamp01(value);
            }
        }

        public void Awake(){
            score= 0;
            attributeValue = attributeInitialValue;
            switch (attributeType) {
                case AttributeType.Health:
                    GameEvents.GetInstance().OnHealthChanged += UpdateAttribute;
                    break;
                case AttributeType.Stamina:
                    GameEvents.GetInstance().OnStaminaChanged += UpdateAttribute;
                    break;
                default :
                    //do nothing
                    break;
            }
            
        }

        //called from Game Event
        public void UpdateAttribute (float newAttributeValue) {
            this.attributeValue = newAttributeValue;
        }

        public float ScoreConsideration()
        {
            score = responseCurve.Evaluate(Mathf.Clamp01(attributeValue / attributeMax));
            return score;
        }
        //public abstract float ScoreConsideration(NPCController npc);
    }

    public enum AttributeType{
        Health,
        Stamina
    }

}
