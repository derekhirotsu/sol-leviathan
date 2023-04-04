using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/RetreatMove")]
public class StateDroneRetreatMove : State
{
    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;

        // don't trigger additional retreating behaviour while retreating
        combat.WillRetreat = false;
        combat.RecoveryMovements++;
        
        enemyFrame.motor.AddSpeedModifier(2f);
    }

    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        CheckTargetDistance(enemyFrame);
    }

    public override void OnStateExit(StateMachine frame) {
        base.OnStateExit(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        enemyFrame.motor.RemoveSpeedModifier(2f);
    }

    // -----
    // Transitions
    // -----

    protected void CheckTargetDistance(EnemyStateMachine enemyFrame) {
        EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;
        Vector3 targetPosition = enemyFrame.motor.agent.destination;
        Vector3 dronePosition = enemyFrame.transform.position;
        bool targetIsClose = Utils.WithinRangeSqr(dronePosition, targetPosition, 1f);

        if (targetIsClose) {
            enemyFrame.StateTransition(combat.DroneStateCollection.RetreatWait);
        }
    }
}
