using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/TurretStateCollection")]
public class TurretStateCollection : EnemyStateCollection
{
    [Header("Unique Turret States")]
    [SerializeField] public State attack;
    [SerializeField] public State reposition;
}
