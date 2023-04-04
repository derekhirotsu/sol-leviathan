using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/Idle")]
public class StateDroneIdle : State
{
    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

         enemyFrame.motor.agent.updateUpAxis = false;
    }

    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        // CheckForTarget(enemyFrame);
    }

    // -----
    // Transitions
    // -----

    protected void CheckForTarget(EnemyStateMachine enemyFrame) {
        if (enemyFrame.view.Target != null) {
            EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;

            enemyFrame.StateTransition(combat.DroneStateCollection.GetPositionNearTarget);
        }
    }
}
