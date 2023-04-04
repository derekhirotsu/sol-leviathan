using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Attack Cooldown")]
public class state_Enemy_AttackCooldown : State
{
    public override void OnStateEnter(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        enemyFrame.combat.canAttack = false;
    }

    public override void OnStateExit(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        enemyFrame.combat.canAttack = true;
    }
}
