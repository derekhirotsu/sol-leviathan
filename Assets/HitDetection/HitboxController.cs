using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitDetection {
    public class HitboxController: MonoBehaviour {
        // -----
        // Adjustable Fields
        // -----

        [SerializeField]
        protected float onHitRecoveryTime;

        // -----
        // HitboxController internal state
        // -----

        protected bool canBeHit = true;

        protected Dictionary<string, Hitbox> hitboxes = new Dictionary<string, Hitbox>();

        protected IEnumerator hitRecoveryCoroutine;

        // -----
        // HitboxController events
        // -----

        public event Action<HitInfo> OnHitboxHit;
        public event Action<HitInfo> OnHitboxInvulnerableHit;
        public event Action OnHitRecoveryEnter;
        public event Action OnHitRecoveryExit;

        // -----
        // Unity Lifecycle methods
        // -----
        
        void Awake() {
            SetInitialHitboxes();
        }

        // -----
        // HitboxController API
        // -----

        public void NotifyHit(HitInfo hitInfo) {
            if (!canBeHit) {
                return;
            }

            if (hitRecoveryCoroutine != null) {
                StopCoroutine(hitRecoveryCoroutine);
            }

            if (hitInfo.DamageSource.ShouldOverrideHitRecovery) {
                hitRecoveryCoroutine = HitRecovery(hitInfo.DamageSource.OverrideHitRecoveryTime);
                StartCoroutine(hitRecoveryCoroutine);
            } else if (onHitRecoveryTime > 0) {
                hitRecoveryCoroutine = HitRecovery(onHitRecoveryTime);
                StartCoroutine(hitRecoveryCoroutine);
            }

            switch (hitInfo.EfficacyBehaviour) {
                case EfficacyBehaviour.Tangible:
                    OnHitboxHit?.Invoke(hitInfo);
                    break;
                case EfficacyBehaviour.Invulnerable:
                    OnHitboxInvulnerableHit?.Invoke(hitInfo);
                    break;
                case EfficacyBehaviour.Intangible:
                    break;
            }
        }
        
        public bool RegisterHitbox(Hitbox hitbox) {
            if (hitboxes.ContainsKey(hitbox.Slug)) {
                string message = $"A Hitbox with this slug already exists: {hitbox.Slug}";
                Debug.LogWarning(message, this);

                return false;
            }
            
            hitboxes.Add(hitbox.Slug, hitbox);
            return true;
        }

        public bool UnregisterHitbox(Hitbox hitbox) {
            if (!hitboxes.ContainsKey(hitbox.Slug)) {
                return false;
            }
            
            hitboxes.Remove(hitbox.Slug);
            return true;
        }

        public Hitbox GetHitbox(string slug) {
            if (hitboxes.ContainsKey(slug)) {
                return hitboxes[slug];
            }

            return null;
        }

        public List<Hitbox> GetHitboxes() {
            return new List<Hitbox>(hitboxes.Values);
        }

        public List<Hitbox> GetHitboxes(string group) {
            List<Hitbox> values = new List<Hitbox>(hitboxes.Values);

            return values.FindAll(hitbox => hitbox.Group == group);
        }

        public void SetHitboxActive(string slug, bool enabled) {
            Hitbox hitbox = GetHitbox(slug);

            if (hitbox != null) {
                hitbox.gameObject.SetActive(enabled);
            }
        }

        public void SetHitboxGroupActive(string group, bool enabled) {
            List<Hitbox> matchingHitboxes = GetHitboxes(group);

            foreach (Hitbox hitbox in matchingHitboxes) {
                hitbox.gameObject.SetActive(enabled);
            }
        }

        public void SetAllHitboxesActive(bool enable) {
            foreach(Hitbox hitbox in hitboxes.Values) {
                hitbox.gameObject.SetActive(enable);
            }
        }

        // -----
        // Protected Methods
        // -----

        protected IEnumerator HitRecovery(float recoveryTime) {
            canBeHit = false;
            OnHitRecoveryEnter?.Invoke();

            yield return new WaitForSeconds(recoveryTime);

            canBeHit = true;
            OnHitRecoveryExit?.Invoke();
        }

        protected void SetInitialHitboxes() {
            Hitbox[] foundHitboxes = GetComponentsInChildren<Hitbox>();

            foreach (var hitbox in foundHitboxes) {
                // Alternatively could give a direct reference through a serialized field.
                hitbox.SetHitboxController(this);
            }
        }
    }
}

