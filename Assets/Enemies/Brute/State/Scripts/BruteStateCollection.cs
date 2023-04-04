using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/Enemy/Wendigo/BruteStateCollection")]
public class BruteStateCollection : EnemyStateCollection
{
    [Header("Brute Combat States")]
    [SerializeField] protected State lightningChannelState;
    public State LightningChannelState { get { return lightningChannelState; } }
    [SerializeField] protected State seekerChannelState;
    public State SeekerChannelState { get { return seekerChannelState; } }
}
