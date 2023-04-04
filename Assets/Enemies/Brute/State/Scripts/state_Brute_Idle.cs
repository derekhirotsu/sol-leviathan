using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Brute/Idle")]
public class state_Brute_Idle : State
{
    [SerializeField] protected float seekerBarrageRange = 7.5f;
    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);

        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemyBruteCombat brute = enemyFrame.combat as EnemyBruteCombat;

        if (Utils.WithinRangeSqr(enemyFrame.transform.position, enemyFrame.combat.PlayerPosition, seekerBarrageRange) ) {
            brute.CastSeekerBarrage();
            frame.StateTransition(brute.BruteStateCollection.SeekerChannelState);

        } else {
            brute.CastLightningStorm();
            frame.StateTransition(brute.BruteStateCollection.LightningChannelState);
        }
        
        
    }
}
