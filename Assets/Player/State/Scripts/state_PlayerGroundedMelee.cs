using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/MeleeStrike")]
public class state_PlayerGroundedMelee : State
{

    [Header("Strike Tuning")]
    [SerializeField] protected float speedReductionWhileStriking = 0.5f;
    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.AddSpeedModifier(-speedReductionWhileStriking);
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);

        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.AddSubstate(playerFrame.StateCollection.MeleeCooldown);

        // Locomotion
        playerFrame.controller.Move();
        
    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.RemoveSpeedModifier(-speedReductionWhileStriking);
    }
}
