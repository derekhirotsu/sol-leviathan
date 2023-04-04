using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Rushdown/GroundedMelee")]
public class state_Rushdown_GroundedMelee : State
{
    
    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        enemyFrame.AddSubstate(enemyFrame.combat.BaseStateCollection.attackCooldown);
    }
}
