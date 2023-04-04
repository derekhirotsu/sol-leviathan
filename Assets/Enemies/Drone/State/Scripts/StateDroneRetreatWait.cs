using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/RetreatWait")]
public class StateDroneRetreatWait : State
{
    [Header ("Test Angle Casting")]
    [Range (0, 360)] [SerializeField] protected float optimalAngleRange = 360;
    [SerializeField] protected Vector2 optimalCastDirection = Vector2.up;
    [SerializeField] protected float optimalLeniency = 2f;
    [SerializeField] protected float minimumCastLength = 2f;
    [SerializeField] protected float maximumCastLength = 4f;

    Vector3 nextLocation;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;

        enemyFrame.motor.agent.isStopped = true;
        enemyFrame.motor.agent.velocity = Vector3.zero;

        if (combat.RecoveryMovements >= 3) {
            combat.WillRetreat = true;
            combat.RecoveryMovements = 0;
            enemyFrame.StateTransition(combat.DroneStateCollection.idle);
            return;
        }

        SetNextDestination(enemyFrame);
        enemyFrame.motor.MoveTo(nextLocation);
    }

    public override void OnStateExit(StateMachine frame) {
        base.OnStateExit(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        enemyFrame.motor.agent.isStopped = false;
    }
    
    // -----
    // Actions
    // -----

    protected void SetNextDestination(EnemyStateMachine enemyFrame) {
        Vector3 targetPosition = enemyFrame.transform.position;

        nextLocation = enemyFrame.motor.GetNavmeshPositionFromAngle(targetPosition, optimalCastDirection, optimalAngleRange, maximumCastLength, minimumCastLength, optimalLeniency, 8);
    }
}
