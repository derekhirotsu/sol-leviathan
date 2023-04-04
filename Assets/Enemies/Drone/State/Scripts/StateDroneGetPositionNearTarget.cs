using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/GetPositionNearTarget")]
public class StateDroneGetPositionNearTarget : State
{
    [Header ("Test Angle Casting")]
    [Range (0, 360)] [SerializeField] protected float optimalAngleRange = 120;
    [SerializeField] protected Vector2 optimalCastDirection = Vector2.up;
    [SerializeField] protected float optimalLeniency = 2f;
    [SerializeField] protected float minimumCastLength = 3f;
    [SerializeField] protected float maximumCastLength = 7f;

    Vector3 nextLocation;

    public bool testDir;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemyDroneCombat combat = enemyFrame.combat as EnemyDroneCombat;

        SetNextDestination(enemyFrame);

        enemyFrame.motor.MoveTo(nextLocation);

        // enemyFrame.StateTransition(combat.DroneStateCollection.DecideMovement);
    }

    // -----
    // Actions
    // -----

    protected void SetNextDestination(EnemyStateMachine enemyFrame) {
        Vector3 targetPosition = enemyFrame.view.Target.position;
        Vector2 evasionDirection = optimalCastDirection;

        if (testDir) {
            Vector3 diff = -enemyFrame.combat.EnemyToPlayerVector;

            evasionDirection.x = diff.x;
            evasionDirection.Normalize();
        }

        nextLocation = enemyFrame.motor.GetNavmeshPositionFromAngle(targetPosition, evasionDirection, optimalAngleRange, maximumCastLength, minimumCastLength, optimalLeniency, 8);
        if (nextLocation.Equals(targetPosition)) {
            nextLocation = enemyFrame.motor.GetNavmeshPositionFromAngle(targetPosition, -Vector2.up, 270f-optimalAngleRange, maximumCastLength / 2, minimumCastLength, optimalLeniency, 8);
        }
    }
}
