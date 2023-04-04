using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Idle")]
public class EnemyIdle : State
{
    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);
    }
    
    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        if (enemyFrame.view.Target != null) {
            // enemyFrame.StateTransition(enemyFrame.stateCollection.chase);
            enemyFrame.StateTransition(enemyFrame.combat.BaseStateCollection.ambush);
        }

        // enemyFrame.motor.agent.enabled = false;
        // enemyFrame.motor.enabled = false;
        // enemyFrame.physics.AddForce(new Vector3(0f, 50f, 0f));
        // if (Keyboard.current.anyKey.wasPressedThisFrame) {
        // }
    }
}
