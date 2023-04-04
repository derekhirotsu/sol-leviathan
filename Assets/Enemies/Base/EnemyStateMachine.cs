using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    // -----
    // Component references
    // -----

    [HideInInspector] public EnemyLineOfSight view;
    [HideInInspector] public HitDetection.HealthController health;
    [HideInInspector] public HitDetection.HitboxController hitboxController;
    [HideInInspector] public EnemyCombat combat;
    [HideInInspector] public EnemyMotor motor;
    [HideInInspector] public PhysicsModule physics;
    [HideInInspector] public EnemyAnimationManager animationManager;

    // -----
    // Unity Lifecycle methods
    // -----
    void Awake() {
        combat = GetComponent<EnemyCombat>();
        motor = GetComponent<EnemyMotor>();
        view = GetComponent<EnemyLineOfSight>();
        health = GetComponent<HitDetection.HealthController>();
        hitboxController = GetComponent<HitDetection.HitboxController>();
        physics = this.GetComponent<PhysicsModule>();
        animationManager = this.GetComponent<EnemyAnimationManager>();
    }

    // -----
    // StateMachine API
    // -----
    public override void Activate() {
        ReturnToIdle();
        active = true;
    }

    public override void Deactivate() {
        ReturnToIdle();
        active = false;
    }
}
