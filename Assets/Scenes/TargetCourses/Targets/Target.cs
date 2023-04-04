using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitDetection;

public class Target : MonoBehaviour {
    protected HealthController healthController;
    protected HitboxController hitboxController;

    [SerializeField]
    protected TargetAudio targetAudio;
    
    [SerializeField]
    protected GameObject destructionEffect;

    void Awake() {
        healthController = GetComponent<HealthController>();
        hitboxController = GetComponent<HitboxController>();
    }

    void OnEnable() {
        hitboxController.OnHitboxHit += HandleHit;
        healthController.OnHealthDepleted += HandleHealthDepleted;
    }

    void OnDisable() {
        hitboxController.OnHitboxHit -= HandleHit;
        healthController.OnHealthDepleted -= HandleHealthDepleted;
    }

    void HandleHit(HitInfo info) {
        targetAudio.PlayTargetHitAudio();
        healthController.TakeDamage(info);
    }

    void HandleHealthDepleted() {
        targetAudio.PlayTargetDestoyedAudio();
        Instantiate(destructionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


}
