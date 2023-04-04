using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/DecideMovement")]
public class StateDroneDecideMovement : State
{
    // [SerializeField] protected float closeDistanceLimit = 7f;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        CheckTargetDistance(enemyFrame);
    }

    // -----
    // Transitions
    // -----

    protected void CheckTargetDistance(EnemyStateMachine enemyFrame) {
        EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;
        Vector3 targetPosition = enemyFrame.view.Target.position;
        Vector3 dronePosition = enemyFrame.transform.position;
        bool targetIsClose = Utils.WithinRangeSqr(dronePosition, targetPosition, combat.CloseDistanceLimit);

        if (targetIsClose) {
            enemyFrame.StateTransition(combat.DroneStateCollection.nearbyChase);
        } else {
            enemyFrame.StateTransition(combat.DroneStateCollection.chase);
        }
    }
}
