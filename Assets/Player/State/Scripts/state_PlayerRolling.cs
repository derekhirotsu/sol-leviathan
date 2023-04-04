using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Player/Rolling")]
public class state_PlayerRolling : State
{
    protected bool rollMeleeTrigger = false;

    [Header("Dodge Config")]
    [SerializeField] protected float rollInvincibilityFrameDuration = 0.8f;

    public override void OnStateEnter(StateMachine frame)
    {
        base.OnStateEnter(frame);

        PlayerStateMachine playerFrame = frame as PlayerStateMachine;
        // playerFrame.health.DodgeForDuration(rollInvincibilityFrameDuration);
        playerFrame.combat.DodgeRollForDuration(rollInvincibilityFrameDuration);
        
        rollMeleeTrigger = false;
    }

    public override void Listen(StateMachine frame)
    {
        if (rollMeleeTrigger) {
            return;
        }

        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;
        playerFrame.AddSubstate(playerFrame.StateCollection.RollCooldown);

        // Get Last-Queued Input
        string queuedInput = playerFrame.inputBuffer.PeekQueuedInput();

        // Pressed the Melee Key
        // if (queuedInput.Equals(InputName.Melee) && playerFrame.melee.CanSlash && playerFrame.melee.RollAttackAvailable) {

        string meleeInput = playerFrame.inputBuffer.GamepadInputActive ? InputName.Melee : InputName.Fire;

        if (queuedInput.Equals(/*InputName.Melee*/meleeInput) && playerFrame.meleeCombat.CanSlash && playerFrame.meleeCombat.RollAttackAvailable) {
            playerFrame.inputBuffer.PopQueuedInput();
            this.restingState = playerFrame.StateCollection.GroundedSpear;
            rollMeleeTrigger = true;
        }

    }

    public override void OnStateExit(StateMachine frame)
    {
        base.OnStateEnter(frame);

        PlayerStateMachine playerFrame = (PlayerStateMachine)frame;

        // // Pressed the Melee Key during roll state
        if (rollMeleeTrigger) {
            // playerFrame.melee.RollAttack();
            playerFrame.meleeCombat.RollAttack();

            // if (!playerFrame.controller.PlayerIsOnGround) {
            //     Debug.Log("zoooooom");
            //     playerFrame.physics.NeutralizeVerticalForce();
            //     playerFrame.physics.AddForce(playerFrame.controller.FacingVector * 15000);
            // }
        }

        this.restingState = playerFrame.StateCollection.GroundedIdle;
        rollMeleeTrigger = false;

    }

    
}
