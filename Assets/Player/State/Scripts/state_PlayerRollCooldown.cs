using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="States/Player/RollCooldown")]
public class state_PlayerRollCooldown : State
{

    [Header("Roll Tuning")]
    [SerializeField] protected float speedReductionWhileRecovering = 0.5f;

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.RemoveSubstate(playerFrame.StateCollection.MeleeCooldown);
        
        playerFrame.controller.AddSpeedModifier(-speedReductionWhileRecovering);
        playerFrame.controller.CanRoll = false;
    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.RemoveSpeedModifier(-speedReductionWhileRecovering);
        playerFrame.controller.CanRoll = true;


    }
}
