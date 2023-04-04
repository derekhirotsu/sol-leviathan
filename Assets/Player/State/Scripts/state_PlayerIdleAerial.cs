using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/IdleAerial")]
public class state_PlayerIdleAerial : State
{

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        playerFrame.combat.DrawGun();
        playerFrame.animator.Play("Jumping");
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        // Locomotion
        playerFrame.controller.Move();
        playerFrame.controller.FaceBasedOnMovement();

        // Get Last-Queued Input
        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        string meleeInput = playerFrame.inputBuffer.GamepadInputActive ? InputName.Melee : InputName.Fire;

        // Holding the Brace Action
        // if (playerFrame.inputBuffer.ActionHeld(InputName.Brace) && playerFrame.blaster.AirAimAvailable && playerFrame.blaster.HoverAvailable) {
        if (playerFrame.inputBuffer.ActionHeld(InputName.Brace) && playerFrame.rangedCombat.AirAimAvailable && playerFrame.rangedCombat.HoverAvailable) {
            playerFrame.StateTransition(playerFrame.StateCollection.AerialAimingBlaster);
        }

        // Pressed down the the Melee Key
        // else if (playerFrame.inputBuffer.ActionHeld(InputName.Melee) && (playerFrame.melee.AirAttackAvailable || playerFrame.melee.GroundSlamAvailable)) {
            
        else if (playerFrame.inputBuffer.ActionHeld(/*InputName.Melee*/ meleeInput) && (playerFrame.meleeCombat.AirAttackAvailable || playerFrame.meleeCombat.GroundSlamAvailable)) {
            playerFrame.StateTransition(playerFrame.StateCollection.AerialMeleeCharge);
        }

        // Landing On Ground
        if (playerFrame.controller.PlayerIsOnGround) {
            playerFrame.StateTransition(playerFrame.StateCollection.GroundedIdle);
            playerFrame.playerAudio.PlayClipOneShot("landing");
        }

    }
}
