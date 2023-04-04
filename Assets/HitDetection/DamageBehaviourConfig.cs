using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitDetection {
    [CreateAssetMenu(menuName = "Damage/Damage Behaviour Config")]
    public class DamageBehaviourConfig : ScriptableObject {
        // -----
        // Adjustable Fields
        // -----

        [SerializeField]
        protected List<DamageEfficacy> damageEfficacies;
        public List<DamageEfficacy> DamageEfficacies { get { return damageEfficacies; } }

        [SerializeField]
        protected float defaultDamageMulitplier;
        public float DefaultDamageMultiplier { get { return defaultDamageMulitplier; } }

        [SerializeField]
        protected EfficacyBehaviour defaultEfficacyBehaviour;
        public EfficacyBehaviour DefaultEfficacyBehaviour { get { return defaultEfficacyBehaviour; } }
    }
}
