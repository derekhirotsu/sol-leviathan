using UnityEngine;

namespace HitDetection {
    [System.Serializable]
    public class DamageEfficacy {
        // Type of damage that this efficacy is concerned with.
        [SerializeField]
        protected DamageType damageType;
        public DamageType DamageType { get { return damageType; } }

        // How effective the damage type is.
        [SerializeField]
        protected float damageMultiplier;
        public float DamageMulitplier { get { return damageMultiplier; } }
        
        [SerializeField]
        protected EfficacyBehaviour efficacyBehaviour;
        public EfficacyBehaviour EfficacyBehaviour { get { return efficacyBehaviour; } }

        public DamageEfficacy(DamageType type, float multiplier, EfficacyBehaviour behavior) {
            damageType = type;
            damageMultiplier = multiplier;
            efficacyBehaviour = behavior;
        }
    }
}
