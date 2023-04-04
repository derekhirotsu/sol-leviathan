using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/AerialMeleeCharge")]
public class state_PlayerAerialMeleeCharge : State
{
    Vector2 aerialPositionDelta = Vector2.zero; // Used to determine what attack is performed

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);
        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        playerFrame.combat.DrawBlade();
        playerFrame.animator.Play("AerialMeleeCharge");
    }

    public override void Listen(StateMachine frame)
    {
        base.Listen(frame);

        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        // Locomotion
        playerFrame.controller.Move();

        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        string meleeInput = playerFrame.inputBuffer.GamepadInputActive ? InputName.Melee : InputName.Fire;

        // If melee button released
        if (!playerFrame.inputBuffer.ActionHeld(/*InputName.Melee*/ meleeInput)) {
            
            // if (playerFrame.melee.AirAttackAvailable) {
            if (playerFrame.meleeCombat.AirAttackAvailable) {
                // If ascending
                Vector3 forwardFacing = playerFrame.controller.FacingVector;
                playerFrame.StateTransition(playerFrame.StateCollection.AerialMelee);
                // playerFrame.melee.AerialSlash();
                playerFrame.meleeCombat.AirMelee();
            } else {
                playerFrame.StateTransition(playerFrame.StateCollection.AerialIdle);
            }
            
        }

        // Landing On Ground
        if (playerFrame.controller.PlayerIsOnGround) {

            // If melee button is still held on landing
            // if (playerFrame.inputBuffer.ActionHeld(InputName.Melee) && playerFrame.melee.GroundSlamAvailable) {
            if (playerFrame.inputBuffer.ActionHeld(/*InputName.Melee*/ meleeInput) && playerFrame.meleeCombat.GroundSlamAvailable) {
                // playerFrame.melee.PlungingAttack();
                playerFrame.meleeCombat.GroundSlam();
                playerFrame.StateTransition(playerFrame.StateCollection.PlungingAttack);
            }

            // If melee is not held, we transition back to an idle grounded state
            else {
                playerFrame.StateTransition(playerFrame.StateCollection.GroundedIdle);
                playerFrame.animator.Play("IdleGrounded");
            }
            
        }
    }
}
