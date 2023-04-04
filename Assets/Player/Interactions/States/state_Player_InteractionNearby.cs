using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Interactions/NearbyInteraction")]
public class state_Player_InteractionNearby : State
{
    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);

        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        // Get Last-Queued Input
        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        // Pressed Jump Key
        if (playerFrame.controller.PlayerIsOnGround && queuedInput.Equals(InputName.Jump) && playerFrame.interactions) {
            playerFrame.StateTransition(playerFrame.interactions.InteractionInProgressState);
            playerFrame.inputBuffer.PopQueuedInput();
            playerFrame.interactions.OnInteract();
        }
    }
}
