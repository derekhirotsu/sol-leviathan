using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Turret/Attack")]
public class StateTurretAttack : State
{
    // EnemyStateMachine stateFrame;
    [SerializeField] protected float meleeDistance = 5f;
    [SerializeField] protected float repositionDistance = 20f;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);
        // stateFrame = (EnemyStateMachine)frame;
    }
    
    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        TurretStateCollection collection = (TurretStateCollection)enemyFrame.combat.BaseStateCollection;
        EnemyTurretCombat combat = (EnemyTurretCombat)enemyFrame.combat;

        if (enemyFrame.view.Target == null) {
            enemyFrame.StateTransition(collection.idle);
            return;
        }
        
        if (Vector3.Distance(enemyFrame.transform.position, enemyFrame.view.Target.position) >= repositionDistance) {
            enemyFrame.StateTransition(collection.reposition);
            return;
        }


        if (Vector3.Distance(enemyFrame.transform.position, enemyFrame.view.Target.position) <= meleeDistance) {
            if (combat.canAttack && enemyFrame.view.HasLineOfSight) {
                combat.MeleeAttack(enemyFrame);
            }
        } else {
            if (combat.canAttack && enemyFrame.view.HasLineOfSight) {
                combat.RangedAttack(enemyFrame);
            }
        }
    }

    public override void OnStateExit(StateMachine frame) {
        base.OnStateExit(frame);
    }
}
