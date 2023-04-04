using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/Enemy/Ambush")]
public class EnemyAmbushState : State
{
    EnemyStateMachine enemyFrame;

    // how close should the enemy be before abandoning the ambush state
    // and moving directly to the player.
    [SerializeField] float rushdownDistance;
    Vector3 ambushLocation;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);
        enemyFrame = (EnemyStateMachine)frame;

        Debug.Log("entering state - finding new ambush point");

        ambushLocation = enemyFrame.motor.GetAmbushPosition(enemyFrame.view.Target.position, 10f, 2f, NavMesh.GetAreaFromName("Walkable"), 0);
    }

    public override void Listen(StateMachine frame) {
        if (Vector3.Distance(enemyFrame.view.Target.position, enemyFrame.transform.position) < rushdownDistance) {
            enemyFrame.StateTransition(enemyFrame.combat.BaseStateCollection.chase);
        }
        
        // if (enemyFrame.motor.agent.remainingDistance < 2f) {
        //     Debug.Log("finding new ambush point");
        //     ambushLocation = enemyFrame.motor.GetAmbushPosition(enemyFrame.view.Target.position, 10f, 2f);

        // }
        enemyFrame.motor.MoveTo(ambushLocation);
    }

    public override void OnStateExit(StateMachine frame) {
    //    enemyFrame.motor.HasValidAmbushPosition = false;
    }
}
