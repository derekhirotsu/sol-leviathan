using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/IdleGrounded")]
public class state_PlayerIdleGrounded : State {

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        playerFrame.meleeCombat.RecoverAirAttacks();
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        // if (playerFrame.blaster.CanRechargeAirShot) {
        if (playerFrame.rangedCombat.CanRechargeAirShot) {
            // playerFrame.blaster.StartAirShotRecharge();
            playerFrame.rangedCombat.StartAirShotRecharge();
        }

        // if (playerFrame.blaster.CanRechargeHover) {
        if (playerFrame.rangedCombat.CanRechargeHover) {
            // playerFrame.blaster.StartHoverRecharge();
            playerFrame.rangedCombat.StartHoverRecharge();
        }

        // if (playerFrame.blaster.IsAirAimInterrupted) {
        if (playerFrame.rangedCombat.IsAirAimInterrupted) {
            // playerFrame.blaster.RestoreAirAim();
            playerFrame.rangedCombat.RestoreAirAim();
        }

        // Locomotion
        playerFrame.controller.Move();
        if (!playerFrame.Contains(playerFrame.StateCollection.MeleeCooldown)) {
            playerFrame.controller.FaceBasedOnMovement();
        }

        // Get Last-Queued Input
        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        // Pressed Roll Key
        if (queuedInput.Equals(InputName.Roll) && playerFrame.controller.CanRoll) {
            playerFrame.StateTransition(playerFrame.StateCollection.Rolling);
            playerFrame.inputBuffer.PopQueuedInput();
            playerFrame.controller.Roll();
        }

        string meleeInput = playerFrame.inputBuffer.GamepadInputActive ? InputName.Melee : InputName.Fire;
        
        if (queuedInput.Equals(meleeInput) && playerFrame.meleeCombat.CanSlash) {
            playerFrame.inputBuffer.PopQueuedInput();
            playerFrame.meleeCombat.GroundMelee();
            playerFrame.StateTransition(playerFrame.StateCollection.GroundedMelee);
        }

        // Pressed the Melee Key
        /*else if (queuedInput.Equals(InputName.Melee) && playerFrame.meleeCombat.CanSlash) {
            // if (frame.Contains(playerFrame.StateCollection.RollCooldown)) {
            //     playerFrame.inputBuffer.PopQueuedInput();
            //     playerFrame.melee.RollAttack();
            //     playerFrame.StateTransition(playerFrame.StateCollection.GroundedSpear);
            // } else {    
                playerFrame.inputBuffer.PopQueuedInput();
                playerFrame.meleeCombat.GroundMelee();
                playerFrame.StateTransition(playerFrame.StateCollection.GroundedMelee);
            // }
        }*/

        // Pressed the Jump Key
        /*else*/ if (queuedInput.Equals(InputName.Jump)) {
            playerFrame.inputBuffer.PopQueuedInput();
            playerFrame.controller.Jump();
            playerFrame.rangedCombat.InterruptBlasterFire();
            playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
        }

        // Holding the Brace Action
        if (playerFrame.inputBuffer.ActionHeld(InputName.Brace)) {
            playerFrame.StateTransition(playerFrame.StateCollection.GroundedAimingBlaster);
        }

        // Leaving the Ground
        if (!playerFrame.controller.PlayerIsOnGround) {
            playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
        }

    }
}
