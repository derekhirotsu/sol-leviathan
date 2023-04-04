using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/Enemy/StateCollection")]
public class EnemyStateCollection : ScriptableObject
{
    [Header("Base Enemy States")]
    [SerializeField] public State idle;
    [SerializeField] public State chase;
    [SerializeField] public State nearbyChase;
    [SerializeField] public State attackCooldown;
    [SerializeField] public State ambush;

    
}
