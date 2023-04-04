using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/PlungingAttack")]
public class state_PlayerPlungingAttack : State
{
    [Header("Plunge Tuning")]
    [SerializeField] protected float speedReductionWhilePlunging = 0.5f;
    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.AddSpeedModifier(-speedReductionWhilePlunging);
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.Move();
    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.RemoveSpeedModifier(-speedReductionWhilePlunging);
    }
}
