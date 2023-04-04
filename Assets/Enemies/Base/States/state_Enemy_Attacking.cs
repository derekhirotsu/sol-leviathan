using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Attacking")]
public class state_Enemy_Attacking : State
{
    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        enemyFrame.AddSubstate(enemyFrame.combat.BaseStateCollection.attackCooldown);
        
    }
}
