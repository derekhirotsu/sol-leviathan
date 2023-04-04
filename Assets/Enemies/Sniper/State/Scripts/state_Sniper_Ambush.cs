using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/Sniper/Ambush")]
public class state_Sniper_Ambush : State
{
    [Header ("Test Angle Casting")]
    [Range (0, 360)] [SerializeField] protected float optimalAngleRange = 180;
    [SerializeField] protected Vector2 optimalCastDirection = Vector2.up;
    [SerializeField] protected float optimalLeniency = 2f;
    [SerializeField] protected float minimumCastLength = 15f;
    [SerializeField] protected float maximumCastLength = 25f;

    [Header ("Combat Fields")]
    [SerializeField] protected float optimalFiringDistance = 10f;

    public override void OnStateEnter(StateMachine frame) {
        base.OnStateEnter(frame);
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemySniperCombat sniper = enemyFrame.combat as EnemySniperCombat;

        Debug.Log("entering state - finding new ambush point");
        enemyFrame.view.SetTargetToPlayer();
        sniper.InterruptAttack();

    }

    public override void Listen(StateMachine frame)
    {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        EnemySniperCombat sniper = enemyFrame.combat as EnemySniperCombat;

        // 1. Get the current player position
        Vector3 targetPos = enemyFrame.view.Target.position;
        Vector3 ambushLocation = Vector3.zero;

        // If the player is too close, try to find a better position.
        if (Utils.WithinRangeSqr(enemyFrame.transform.position, targetPos, optimalFiringDistance)) {
            
            if ( enemyFrame.motor.agent.enabled && enemyFrame.motor.agent.remainingDistance < 2) {
                // 2. Check for a location. If none are found above the player, check elsewhere.
                ambushLocation = enemyFrame.motor.GetNavmeshPositionFromAngle(targetPos, optimalCastDirection, optimalAngleRange, maximumCastLength, minimumCastLength, optimalLeniency);
                if (ambushLocation.Equals(targetPos)) {
                    ambushLocation = enemyFrame.motor.GetNavmeshPositionFromAngle(targetPos, Vector2.down, 270f-optimalAngleRange, maximumCastLength / 2, minimumCastLength, optimalLeniency);
                }

                // 3. Move to the ambush position and transition to the locomotion state.
                enemyFrame.motor.MoveTo(ambushLocation);
            }
            
            enemyFrame.StateTransition(sniper.SniperStateCollection.MovingToAmbushPosition);

        } else if (enemyFrame.combat.canAttack) {
            
            enemyFrame.motor.MoveTo(frame.transform.position);
            enemyFrame.StateTransition(sniper.SniperStateCollection.Aiming);

        }
    }

}
