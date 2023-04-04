using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Rushdown/Chase")]
public class state_Rushdown_Chase : State
{
    // [SerializeField] protected float nearChaseDistance;

    public override void Listen(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemyRushdownCombat rushdownCombat = (EnemyRushdownCombat)enemyFrame.combat;

        // Debug.Log(frame.name + " Is Chasing the Player");

        enemyFrame.motor.MoveTo(rushdownCombat.PlayerPosition);

        if (rushdownCombat.PlayerWithinMeleeRange) {

            // enemyFrame.combat.CheckState(enemyFrame);
            rushdownCombat.GroundedMeleeStrike();
            enemyFrame.StateTransition(rushdownCombat.RushdownStateCollection.GroundedMeleeStrike);
            enemyFrame.AddSubstate(enemyFrame.combat.BaseStateCollection.attackCooldown);
        }
    }

}
