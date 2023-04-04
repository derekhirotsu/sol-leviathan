using System;
using UnityEngine;

namespace HitDetection {
    public class HealthController: MonoBehaviour {
        [Header("Health Tracking Fields (Required)")]
        [SerializeField] protected StatProfile profile;
        [SerializeField] protected EntityLookup entityLookup;

        // -----
        // HealthController internal state
        // -----

        protected float currentHealth;
        public float CurrentHealth { get { return currentHealth; } }
        public float CurrentHealthPercentage { get { return (float) currentHealth / profile.MaxHealth; } }
        public virtual float MaxHealth { get { return profile.MaxHealth; } }

        // -----
        // HealthController events
        // -----

        public event Action<HitInfo> OnTakeDamage;
        public event Action OnHealthDepleted;
        public event Action CurrentHealthChange;

        // -----
        // Unity Lifecycle methods
        // -----

        protected virtual void Awake() {
            if (profile == null) {
                enabled = false;
                return;
            }

            currentHealth = MaxHealth;
        }

        protected virtual void OnEnable() {
            if (entityLookup != null) {
                entityLookup.AddItem(this);
            }
        }

        protected virtual void OnDisable() {
            if (entityLookup != null) {
                entityLookup.RemoveItem(this);
            } 
        }

        // -----
        // HealthController API
        // -----
        
        public void SetCurrentHealth(float newValue) {
            currentHealth = Mathf.Clamp(newValue, 0, MaxHealth);
            CurrentHealthChange?.Invoke();

            if (currentHealth <= 0) {
                OnHealthDepleted?.Invoke();
            }
        }

        public void ModifyCurrentHealth(float value) {
            currentHealth = Mathf.Clamp(currentHealth + value, 0, MaxHealth);
            CurrentHealthChange?.Invoke();
            
            if (currentHealth <= 0) {
                OnHealthDepleted?.Invoke();
            }
        }

        public void TakeDamage(HitInfo hitInfo) {
            ModifyCurrentHealth(-hitInfo.DamageDealt);

            if (currentHealth > 0) {
                OnTakeDamage?.Invoke(hitInfo);
            }  
        
        }
    }
}
