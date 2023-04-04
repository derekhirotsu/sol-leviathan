using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Different enemy types will inherit from this base class
// and define what they do the attack method
public abstract class EnemyCombat : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] protected EntityLookup ActivePlayer;
    public Vector3 PlayerPosition {
        get {
            if (ActivePlayer.Items.Count <= 0) {
                return this.transform.position;
            }

            return ActivePlayer.Items[0].transform.position;
        }
    }

    public Vector2 EnemyToPlayerVector {
        get {
            if (ActivePlayer.Items.Count <= 0) {
                return Vector2.zero;
            }

            Vector3 playerVector = (ActivePlayer.Items[0].transform.position - this.transform.position).normalized;
            return playerVector;
        }
    }
    
    // -----
    // State Collection Base Field
    // -----
    [Header("Base State Collection")]
    [SerializeField] protected EnemyStateCollection baseStateCollection;
    public EnemyStateCollection BaseStateCollection { get { return baseStateCollection; } }

    // -----
    // Component references
    // -----
    protected EnemyStateMachine stateMachine;
    public bool canAttack = true;

    public delegate void ActionDelegate(StateMachine frame);
    public ActionDelegate onPerformAction;

    public virtual void Awake() {
        stateMachine = this.GetComponent<EnemyStateMachine>();
    }

    public virtual void Act(StateMachine frame) {
        if (!canAttack) {
            return;
        }
        
        // perform action(s) based on currentState
        if (onPerformAction != null) {
            onPerformAction(frame);
        }
    }

    protected IEnumerator activeAttackCoroutine;
    public virtual void InterruptAttack() {
        if (activeAttackCoroutine != null) {
            StopCoroutine(activeAttackCoroutine);
        }
        activeAttackCoroutine = null;
    }

    // CheckState will be called in an enemy state machine
    // in order to tell the EnemyCombat component what state the
    // enemy is in. Based on the current state the action 
    // delegate will be updated with different attack methods;
    public virtual void CheckState(StateMachine frame) { }

    public virtual Vector3 CreateAttackForceVector(/*Vector3 hitPosition, Vector3 sourcePosition, Vector3 forceImpulse,*/ HitDetection.HitInfo info) {
        Vector3 hitPosition = info.Hitbox.transform.position;
        Vector3 sourcePosition = info.DamageSource.SourceEntity.transform.position;
        Vector3 attackDirection = hitPosition - sourcePosition;
        float xDirection = (attackDirection.normalized).x > 0 ? 1 : -1;
        Vector3 attackForceVector = new Vector2(xDirection, 1f) * info.DamageSource.ForceImpulse;

        return attackForceVector;
    }

    // -----
    // Event Callbacks
    // -----

    public virtual void OnHealthDepleted() {
        Destroy(this.gameObject);
    }

    public virtual void OnTakeDamage(HitDetection.HitInfo info) {      
        Vector3 attackForceVector = CreateAttackForceVector(info);

        stateMachine.physics.NeutralizeVerticalForce();
        stateMachine.motor.ForceTranslation(attackForceVector);
    }

    public virtual void OnHitboxHit(HitDetection.HitInfo info) {
        Debug.Log(info);
        stateMachine.health.TakeDamage(info);
    }

    // -----
    // Event Subscriptions
    // -----
    public virtual void OnEnable() {
        stateMachine.health.OnTakeDamage += OnTakeDamage;
        stateMachine.health.OnHealthDepleted += OnHealthDepleted;
        stateMachine.hitboxController.OnHitboxHit += OnHitboxHit;
    }

    public virtual void OnDisable() {
        stateMachine.health.OnTakeDamage -= OnTakeDamage;
        stateMachine.health.OnHealthDepleted -= OnHealthDepleted;
        stateMachine.hitboxController.OnHitboxHit -= OnHitboxHit;
    }
}
