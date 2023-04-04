using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/Enemy/Wendigo/SniperStateCollection")]
public class SniperStateCollection : EnemyStateCollection
{
    [Header("Sniper Ambush States")]
    [SerializeField] protected State movingToAmbushState;
    public State MovingToAmbushPosition { get { return movingToAmbushState; } }

    [Header("Sniper Combat States")]
    [SerializeField] protected State aiming;
    public State Aiming { get { return aiming; } }

    [SerializeField] protected State firing;
    public State Firing { get { return firing; } }
    
}
