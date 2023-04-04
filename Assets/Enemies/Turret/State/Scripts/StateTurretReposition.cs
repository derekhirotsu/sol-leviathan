using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/Enemy/Turret/Reposition")]
public class StateTurretReposition : State
{
    Vector3 newLocation;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        Debug.Log("entering state - finding new ambush point");
        newLocation = enemyFrame.motor.GetAmbushPosition(enemyFrame.view.Target.position, 10f, 2f, NavMesh.GetAreaFromName("Walkable"), 0);

        enemyFrame.motor.agent.Warp(newLocation);
    }

    public override void Listen(StateMachine frame) {}

    public override void OnStateExit(StateMachine frame) {
        base.OnStateExit(frame);
    }
}
