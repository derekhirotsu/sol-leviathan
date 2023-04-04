using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/MeleeCooldown")]
public class state_PlayerMeleeCooldown : State
{
    [Header("Strike Tuning")]
    [SerializeField] protected float speedReductionWhileStriking = 0.5f;
    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.AddSpeedModifier(-speedReductionWhileStriking);
    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.RemoveSpeedModifier(-speedReductionWhileStriking);
        // playerFrame.melee.RecoverMelee();
        playerFrame.meleeCombat.RecoverMelee();
    }
}
