using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/Chase")]
public class StateDroneChase : State
{
    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        // EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
    
        // enemyFrame.motor.MoveTo(enemyFrame.view.Target.position);
    }

    public override void Listen(StateMachine frame) {
        base.Listen(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        CheckTargetDistance(enemyFrame);

        EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;

        combat.RangedAttack(frame);
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
            enemyFrame.StateTransition(combat.DroneStateCollection.NearbyWait);
            // enemyFrame.motor.agent.isStopped = true;
            // enemyFrame.motor.agent.velocity = Vector3.zero;
        } else {

        }
    }
}
