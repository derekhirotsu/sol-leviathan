using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/Enemy/Wendigo/RushdownStateCollection")]
public class RushdownStateCollection : EnemyStateCollection
{
    [Header("Grounded Melee States")]
    [SerializeField] protected State groundedMeleeStrike;
    public State GroundedMeleeStrike { get { return groundedMeleeStrike; } }

    [Header("Aerial Melee States")]
    [SerializeField] protected State aerialLeaping;
    public State AerialLeaping { get { return aerialLeaping; } }
    
}
