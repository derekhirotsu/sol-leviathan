using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitDetection {
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour {
        // -----
        // Adjustable Fields
        // -----

        [Header("Hitbox Identification")]

        [SerializeField]
        protected string slug;
        public string Slug { get { return slug; } }

        [SerializeField]
        protected HitboxGroup group;
        public string Group {
            get {
                if (group != null) {
                    return group;
                }
                return "";
            }
        }

        [Header("Damage Behaviours")]
        
        [SerializeField]
        protected DamageBehaviourConfig damageBehaviours;

        [Header("Target Entity")]

        [SerializeField] protected GameObject targetEntity;
        public GameObject TargetEntity { get { return targetEntity; } }

        // -----
        // Cached component references
        // -----

        protected HitboxController hitboxController;
        public HitboxController HitboxController { get { return hitboxController; } }

        // -----
        // Hitbox internal state
        // -----
        
        // Tracks how much damage a hitbox has recieved during its lifetime.
        // This can be used to determine behaviour after a certain amount of
        // damage has been done to a specific hitbox.
        protected float accumulatedDamage = 0f;
        public float AccumulatedDamage { get { return accumulatedDamage; } }

        protected Dictionary<DamageType, DamageEfficacy> damageEfficacies;
        
        protected DamageEfficacy defaultDamageEfficacy;

        // -----
        // Unity Lifecycle methods
        // -----

        void Awake() {
            InitTargetEntity();
            InitDamageEfficacyDictionary();
            InitDefaultDamageEfficacy();
        }

        void OnDestroy() {
            if (hitboxController != null) {
                hitboxController.UnregisterHitbox(this);
            }
        }

        // -----
        // Hitbox API
        // -----

        public bool HandleHit(DamageSource source) {
            if (hitboxController == null) {
                return false;
            }

            DamageEfficacy efficacy = GetEfficacyFromDamageType(source.DamageType);
            EfficacyBehaviour behaviour = efficacy.EfficacyBehaviour;

            if (IsIntangible(behaviour)) {
                return false;
            }

            float damage = CalculateDamage(source, efficacy);
            HitInfo info = new HitInfo(this, damage, behaviour, source);

            accumulatedDamage += damage;
            hitboxController.NotifyHit(info);

            return true;
        }

        public void SetHitboxController(HitboxController controller) {
            if (!controller.RegisterHitbox(this)) {
                // Unable to register with given controller; Do nothing further.
                return;
            }

            if (hitboxController != null) {
                hitboxController.UnregisterHitbox(this);
            }

            hitboxController = controller;
        }
        
        public bool IsIntangible(EfficacyBehaviour behaviour) {
            return behaviour == EfficacyBehaviour.Intangible;
        }

        public bool IsIntangible(DamageSource source) {
            DamageEfficacy efficacy = GetEfficacyFromDamageType(source.DamageType);
            EfficacyBehaviour behaviour = efficacy.EfficacyBehaviour;
            
            return behaviour == EfficacyBehaviour.Intangible;
        }

        public bool IsInvulnerable(DamageSource source) {
            DamageEfficacy efficacy = GetEfficacyFromDamageType(source.DamageType);
            EfficacyBehaviour behaviour = efficacy.EfficacyBehaviour;
            
            return behaviour == EfficacyBehaviour.Invulnerable;  
        }

        // -----
        // Protected Methods
        // -----

        protected float CalculateDamage(DamageSource source, DamageEfficacy efficacy) {
            float unboundedDamage = 0f;

            if (efficacy.EfficacyBehaviour == EfficacyBehaviour.Tangible) {
                unboundedDamage = source.RawDamageValue * efficacy.DamageMulitplier;
            }

            return Mathf.Clamp(unboundedDamage, 0f, Mathf.Infinity);
        }

        protected DamageEfficacy GetEfficacyFromDamageType(DamageType damageType) {
            if (damageEfficacies.ContainsKey(damageType)) {
                return damageEfficacies[damageType];
            }

            return defaultDamageEfficacy;
        }
        
        protected void InitTargetEntity() {
            if (targetEntity == null) {
                targetEntity = gameObject;
            }
        }

        protected void InitDamageEfficacyDictionary() {
            damageEfficacies = new Dictionary<DamageType, DamageEfficacy>();

            foreach (DamageEfficacy efficacy in damageBehaviours.DamageEfficacies) {
                DamageType damageType = efficacy.DamageType;

                if (damageEfficacies.ContainsKey(damageType)) {
                    string message = $"Duplicate DamageType {damageType} encountered in list of default damage efficacy behaviours.";
                    Debug.LogWarning(message, this);
                } else {
                    damageEfficacies.Add(damageType, efficacy);
                }
            }
        }

        protected void InitDefaultDamageEfficacy() {
            defaultDamageEfficacy = new DamageEfficacy(null, damageBehaviours.DefaultDamageMultiplier, damageBehaviours.DefaultEfficacyBehaviour);
        }
    }
}
