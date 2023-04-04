using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/AerialAiming")]
public class state_PlayerAerialAiming : State
{
    [Header("Aerial Control Tuning")]
    [SerializeField] protected float speedReductionWhileAiming = 0.6f;

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.AddSpeedModifier(-speedReductionWhileAiming);
        playerFrame.rangedCombat.ToggleAimReticle(true);

    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.rangedCombat.UpdateAimVector();

        // Drain stabilizer and animate
        if (CheckStabilizerAvailable(playerFrame)) {
            ActionDrainStabilizer(playerFrame);
        }

        // Releasing the Brace Action
        if (!CheckIsBraceHeld(playerFrame)) {
            ActionReturnToAerialIdle(playerFrame);
        }

        // Locomotion
        ActionMove(playerFrame);

        if (CheckCanFirePrimaryWeapon(playerFrame)) {
            ActionFirePrimaryWeapon(playerFrame);
        }

        // Pressed the Fire Key
        if (CheckCanFireAerialShot(playerFrame)) {
            ActionFireAerialShot(playerFrame);
        }

        // Landing On Ground
        if (CheckIsGrounded(playerFrame)) {
            ActionLandOnGround(playerFrame);
        }

    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.controller.RemoveSpeedModifier(-speedReductionWhileAiming);
        playerFrame.rangedCombat.ToggleAimReticle(false);

    }

    // -----
    // State conditional checks
    // -----

    protected bool CheckStabilizerAvailable(PlayerStateMachine playerFrame) {
        // return playerFrame.blaster.HoverAvailable;
        return playerFrame.rangedCombat.HoverAvailable;
    }

    protected bool CheckIsBraceHeld(PlayerStateMachine playerFrame) {
        return playerFrame.inputBuffer.ActionHeld(InputName.Brace);
    }

    protected bool CheckCanFirePrimaryWeapon(PlayerStateMachine playerFrame) {
        // return playerFrame.inputBuffer.ActionHeld(InputName.Fire) && !playerFrame.blaster.CurrentlyFiring && playerFrame.blaster.HoverAvailable;
        return playerFrame.inputBuffer.ActionHeld(InputName.Fire) && !playerFrame.rangedCombat.CurrentlyFiring && playerFrame.rangedCombat.HoverAvailable;
    }

    protected bool CheckCanFireAerialShot(PlayerStateMachine playerFrame) {
        // Get Last-Queued Input
        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        // return queuedInput.Equals(InputName.HeavyFire) && playerFrame.blaster.AirShotAvailable && playerFrame.blaster.HoverAvailable;
        return queuedInput.Equals(InputName.HeavyFire) && playerFrame.rangedCombat.AirShotAvailable && playerFrame.rangedCombat.HoverAvailable;
    }

    protected bool CheckIsGrounded(PlayerStateMachine playerFrame) {
        return playerFrame.controller.PlayerIsOnGround;
    }

    // -----
    // State actions
    // -----

    protected void ActionDrainStabilizer(PlayerStateMachine playerFrame) {
        playerFrame.animator.Play("AirAiming");
        playerFrame.physics.NeutralizeVerticalForce();
        // playerFrame.blaster.UseHover();
        playerFrame.rangedCombat.UseHover();

        // if (!playerFrame.blaster.HoverAvailable) {
        if (!playerFrame.rangedCombat.HoverAvailable) {
            playerFrame.rangedCombat.ToggleAimReticle(false);
            playerFrame.animator.Play("Jumping");
        }
    }

    protected void ActionReturnToAerialIdle(PlayerStateMachine playerFrame) {
        playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
    }

    protected void ActionMove(PlayerStateMachine playerFrame) {
        playerFrame.controller.Move();
        playerFrame.controller.FaceBasedOnAim();
    }

    protected void ActionFirePrimaryWeapon(PlayerStateMachine playerFrame) {
        // playerFrame.blaster.FirePrimaryWeapon();
        playerFrame.rangedCombat.FirePrimaryWeapon();
        Vector2 force = playerFrame.controller.AimVector;
        force.x *= 0.68f;
        force = force.normalized;
        playerFrame.physics.AddForce(-(force * 100));
    }

    protected void ActionFireAerialShot(PlayerStateMachine playerFrame) {
        playerFrame.inputBuffer.PopQueuedInput();
        // playerFrame.blaster.FireAerialWeapon();
        playerFrame.rangedCombat.FireAerialWeapon();
        playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
    }

    protected void ActionLandOnGround(PlayerStateMachine playerFrame) {
        playerFrame.StateTransition(playerFrame.StateCollection.GroundedIdle);
        playerFrame.animator.Play("IdleGrounded");
    }
}
