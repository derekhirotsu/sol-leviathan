using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Rushdown/Idle")]
public class state_Rushdown_Idle : State
{
    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        
    }
}
