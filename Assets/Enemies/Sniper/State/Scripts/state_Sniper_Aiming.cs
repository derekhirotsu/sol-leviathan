using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Sniper/Aiming")]
public class state_Sniper_Aiming : State
{
    [Header ("Combat Fields")]
    [SerializeField] protected float meleeDistance = 5f;

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);

        EnemySniperCombat sniper = ((EnemyStateMachine)frame).combat as EnemySniperCombat;
        sniper.DisplayTelegraph(true);
        sniper.InterruptAttack();
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);

        EnemySniperCombat sniper = ((EnemyStateMachine)frame).combat as EnemySniperCombat;
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;

        sniper.AimAtPlayer();

        if (sniper.FullyCharged) {

            if (Utils.WithinRangeSqr(enemyFrame.transform.position, sniper.TargetPosition, meleeDistance)) {
                sniper.MeleeAttack();
                enemyFrame.StateTransition(sniper.SniperStateCollection.Firing);
            
            } else {
                sniper.FireSniper();
                enemyFrame.StateTransition(sniper.SniperStateCollection.Firing);

            }
        }
    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateExit(frame);

        EnemySniperCombat sniper = ((EnemyStateMachine)frame).combat as EnemySniperCombat;
        sniper.DisplayTelegraph(false);

    }
}
