using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/NearbyChase")]
public class EnemyNearbyChase : State
{
    [SerializeField] protected float speedReductionWhileNearby = 0.5f;

    public override void OnStateEnter(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        enemyFrame.motor.AddSpeedModifier(-speedReductionWhileNearby);
    }
    
    // public override void Listen(StateMachine frame) {
    //     EnemyPrototypeStateController enemyFrame = (EnemyPrototypeStateController)frame;

    //     enemyFrame.combat.CheckState(enemyFrame);
    //     enemyFrame.combat.Act(enemyFrame);
    // }

    public override void OnStateExit(StateMachine frame) {
        EnemyStateMachine enemyFrame = (EnemyStateMachine)frame;
        enemyFrame.motor.RemoveSpeedModifier(-speedReductionWhileNearby);
    }
}
