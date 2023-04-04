using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Interactions/InProgress")]
public class state_Player_InteractionInProgress : State
{
    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);

        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.Move();
        
    }
}
