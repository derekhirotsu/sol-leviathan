using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/GroundedAiming")]
public class state_PlayerGroundedAiming : State
{
    [Header("Aim Tuning")]
    [SerializeField] protected float speedReductionWhileAiming = 0.6f;

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.AddSpeedModifier(-speedReductionWhileAiming);
        playerFrame.animator.SetLayerWeight(2, 1.0f);
        playerFrame.animator.Play("IdleGrounded");

        playerFrame.combat.DrawGun();
        playerFrame.rangedCombat.ToggleAimIK(true);
        playerFrame.rangedCombat.ToggleAimReticle(true);
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        // Locomotion
        playerFrame.controller.Move();
        playerFrame.controller.FaceBasedOnAim();
        playerFrame.rangedCombat.UpdateAimVector();

        // Get Last-Queued Input
        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        // Pressed the Fire Key
        // if (playerFrame.inputBuffer.ActionHeld(InputName.Fire) && !playerFrame.blaster.CurrentlyFiring) {
        if (playerFrame.inputBuffer.ActionHeld(InputName.Fire) && !playerFrame.rangedCombat.CurrentlyFiring) {
            // playerFrame.blaster.FirePrimaryWeapon();
            playerFrame.rangedCombat.FirePrimaryWeapon();

        // } else if (playerFrame.inputBuffer.ActionHeld(InputName.HeavyFire) && playerFrame.blaster.MissilesAvailable && !playerFrame.blaster.CurrentlyFiring) {
        } else if (playerFrame.inputBuffer.ActionHeld(InputName.HeavyFire) && playerFrame.rangedCombat.MissilesAvailable && !playerFrame.rangedCombat.CurrentlyFiring) {

            // playerFrame.blaster.FireMissile();
            playerFrame.rangedCombat.FireMissile();

        }

        // Pressed the Jump Key
        if (queuedInput.Equals(InputName.Jump)) {
            playerFrame.inputBuffer.PopQueuedInput();
            playerFrame.controller.Jump();
            playerFrame.rangedCombat.InterruptBlasterFire();
            playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
        }

        // Pressed Roll Key
        else if (queuedInput.Equals(InputName.Roll) && playerFrame.controller.CanRoll) {
            playerFrame.inputBuffer.PopQueuedInput();
            playerFrame.controller.Roll();
            playerFrame.StateTransition(playerFrame.StateCollection.Rolling);
        }

        // Released Brace action
        if (!playerFrame.inputBuffer.ActionHeld(InputName.Brace)) {
            playerFrame.StateTransition(playerFrame.StateCollection.GroundedIdle);
        }

        // Leaving the Ground
        if (!playerFrame.controller.PlayerIsOnGround) {
            playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
        }

    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.animator.SetLayerWeight(2, 0.0f);
        playerFrame.controller.RemoveSpeedModifier(-speedReductionWhileAiming);
        playerFrame.rangedCombat.ToggleAimIK(false);
        playerFrame.rangedCombat.ToggleAimReticle(false);
    }
}
