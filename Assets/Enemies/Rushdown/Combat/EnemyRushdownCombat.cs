using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRushdownCombat : EnemyCombat
{
    protected RushdownStateCollection rushdownStateCollection;
    public RushdownStateCollection RushdownStateCollection { get { return rushdownStateCollection; } }

    public override void Awake() {
        base.Awake();
        rushdownStateCollection = (RushdownStateCollection) baseStateCollection;
    }

    [SerializeField] protected float optimalMeleeRange = 5f;
    public bool PlayerWithinMeleeRange {
        get {
            Transform player = ActivePlayer.Items[0].transform;
            float distanceToPlayer = (player.transform.position - this.transform.position).magnitude;

            if (canAttack && distanceToPlayer < optimalMeleeRange) {
                return true;
            } else {
                // Debug.Log(this.name + " is not within range of the player");
                return false;
            }
            
        }
    }

    // ---
    // Melee Configs
    // ---
    [Header("Melee Configs")]
    // [SerializeField] protected config_MeleeStats groundedStrike;
    [SerializeField]
    protected MeleeAttackConfig groundedStrike;

    protected IEnumerator currentMelee;
    public void GroundedMeleeStrike() {
        if (!canAttack) {
            return;
        }

        // Debug.Log(this.name + " Is attempting a melee against " + ActivePlayer.Items[0].name);
        stateMachine.animationManager.LockAlignment();
        
        canAttack = false;

        if (currentMelee != null) {
            StopCoroutine(currentMelee);
            currentMelee = null;
        }

        currentMelee = MeleeCoroutine(groundedStrike);
        StartCoroutine(currentMelee);
    }

    protected IEnumerator MeleeCoroutine(MeleeAttackConfig stats) {
        Vector2 strikeVector = new Vector2(EnemyToPlayerVector.x, 0).normalized;
        stateMachine.animationManager.PlayAnimation(EnemyAnimations.anim_ActionEnter);

        // Windup
        float windupTime = stats.WindupTime;
        while (windupTime > 0) {
            strikeVector = new Vector2(EnemyToPlayerVector.x, 0).normalized;
            stateMachine.animationManager.AlignModelWithVector(strikeVector);
            
            windupTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        // Execution
        float swingTime = stats.SwingTime;
        stateMachine.animationManager.ActionTrigger();

        while (swingTime > 0) {
            stateMachine.motor.ForceTranslation(strikeVector * stats.MoveSpeedWhileStriking);
            swingTime -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Vector3 forwardOffset = strikeVector * 2;

        stats.Attack(this.gameObject, forwardOffset, strikeVector, parentedToSource:true);

        // Section of the recovery
        float recoveryTimer = stats.RecoveryTime;
        while (recoveryTimer > 0) {
            stateMachine.motor.ForceTranslation(strikeVector * (stats.MoveSpeedWhileStriking * recoveryTimer));
            recoveryTimer -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Recovery
        stateMachine.animationManager.UnlockAlignment();
        currentMelee = null;
    }
}
