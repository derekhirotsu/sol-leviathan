using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/PlayerStateCollection")]
public class PlayerStateCollection : ScriptableObject
{
    [Header("Locomotion")]
    [SerializeField] protected State groundedIdle;
    public State GroundedIdle { get { return groundedIdle; } }

    [SerializeField] protected State aerialIdle;
    public State AerialIdle { get { return aerialIdle; } }
    [SerializeField] protected State rolling;
    public State Rolling { get { return rolling; } }

    [Header("Gun")]
    [SerializeField] protected State groundedAimingBlaster;
    public State GroundedAimingBlaster { get { return groundedAimingBlaster; } }
    [SerializeField] protected State aerialAimingBlaster;
    public State AerialAimingBlaster { get { return aerialAimingBlaster; } }

    [Header("Melee")]
    [SerializeField] protected State groundedMelee;
    public State GroundedMelee { get { return groundedMelee; } }
    [SerializeField] protected State aerialMelee;
    public State AerialMelee { get { return aerialMelee; } }
    [SerializeField] protected State groundedSpear;
    public State GroundedSpear { get { return groundedSpear; } }
    [SerializeField] protected State aerialMeleeCharge;
    public State AerialMeleeCharge { get { return aerialMeleeCharge; } }
    [SerializeField] protected State plungingAttack;
    public State PlungingAttack { get { return plungingAttack; } }

    [Header("Cooldowns")]
    [SerializeField] protected State meleeCooldown;
    public State MeleeCooldown { get { return meleeCooldown; } }
    [SerializeField] protected State rollCooldown;
    public State RollCooldown { get { return rollCooldown; } }
    

}
