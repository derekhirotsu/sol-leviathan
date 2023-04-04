using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Drone/NearbyWait")]
public class StateDroneNearbyWait : State
{
    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        SetupWait(enemyFrame);
    }

    public override void Listen(StateMachine frame) {}

    public override void OnStateExit(StateMachine frame) {
        base.OnStateExit(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        enemyFrame.motor.agent.isStopped = false;
    }

    protected void SetupWait(EnemyStateMachine enemyFrame) {
        enemyFrame.motor.agent.isStopped = true;
        enemyFrame.motor.agent.velocity = Vector3.zero;

        // Vector2 lookDirection = new Vector2(enemyFrame.combat.EnemyToPlayerVector.x, 0).normalized;

        // Debug.DrawRay(enemyFrame.transform.position, lookDirection, Color.gray, 1f);

        // enemyFrame.animationManager.AlignModelWithVector(lookDirection);
    }

}
