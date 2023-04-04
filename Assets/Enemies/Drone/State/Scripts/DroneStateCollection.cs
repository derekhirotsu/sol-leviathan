using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Collections/DroneStateCollection")]
public class DroneStateCollection : EnemyStateCollection
{
  [Header("Drone States")]
  [SerializeField] State decideMovement;
  public State DecideMovement { get { return decideMovement; } }

  [SerializeField] State getPositionNearTarget;
  public State GetPositionNearTarget { get { return getPositionNearTarget; } }

  [SerializeField] State nearbyWait;
  public State NearbyWait { get { return nearbyWait; } }

  [SerializeField] State hitRetreat;
  public State HitRetreat { get { return hitRetreat; } }

  [SerializeField] State retreatWait;
  public State RetreatWait { get { return retreatWait; } }
}
