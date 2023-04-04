using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Turret/Idle")]
public class StateTurretIdle : State
{

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);
    }
    
    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        if (enemyFrame.view.Target != null) {
            TurretStateCollection collection = (TurretStateCollection)enemyFrame.combat.BaseStateCollection;

            enemyFrame.StateTransition(collection.attack);
        }
    }

    public override void OnStateExit(StateMachine frame) {
        base.OnStateExit(frame);
    }
}
