using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDroneCombat : EnemyCombat
{
    protected DroneStateCollection droneStateCollection;
    public DroneStateCollection DroneStateCollection { get { return droneStateCollection; } }

    [Header("Ranged Attack")]
    [SerializeField] protected ProjectileAttackConfig rangedAttack;
    
    [SerializeField] protected float rangedCooldown = 2f;
    public float RangedCooldown { get { return rangedCooldown; } }

    [SerializeField] protected float pesterCooldown = 5f;
    public float PesterCooldown { get { return pesterCooldown; } }

    [SerializeField] protected float pesterInterval = 0.2f;
    public float PesterInterval { get { return pesterInterval; } }

    [SerializeField] protected float closeDistanceLimit = 7f;
    public float CloseDistanceLimit { get { return closeDistanceLimit; } }

    // -----
    // Drone Combat state
    // -----

    protected int recoveryMovements = 0;
    public int RecoveryMovements { get { return recoveryMovements; } set { recoveryMovements = value; } }

    public bool WillRetreat = true;

    // -----
    // Unity Lifecycle methods
    // -----

    public override void Awake() {
        base.Awake();
        droneStateCollection = (DroneStateCollection) baseStateCollection;
    }

    // -----
    // Drone Combat API
    // -----

    public void RangedAttack(StateMachine frame) {
        if (!canAttack) {
            return;
        }

        InterruptAttack();

        activeAttackCoroutine = Ranged(frame);
        StartCoroutine(activeAttackCoroutine);
    }

    // -----
    // Event Callbacks
    // -----
    
    public override void OnTakeDamage(HitDetection.HitInfo info) {
        if (WillRetreat) {
            stateMachine.motor.agent.velocity = Vector3.zero;
            stateMachine.StateTransition(droneStateCollection.HitRetreat);
        }
    }

    // -----
    // Protected Methods
    // -----

    protected IEnumerator Ranged(StateMachine frame) {
        canAttack = false;

        EnemyStateMachine enemyFrame = (EnemyStateMachine) frame;
        Vector3 aimVector = (enemyFrame.view.Target.position - transform.position).normalized;

        rangedAttack.Attack(transform.position + new Vector3(0f, 0.5f, 0f) + (aimVector * 1), aimVector, Quaternion.identity);

        yield return new WaitForSeconds(rangedCooldown);

        canAttack = true;
    }
}
