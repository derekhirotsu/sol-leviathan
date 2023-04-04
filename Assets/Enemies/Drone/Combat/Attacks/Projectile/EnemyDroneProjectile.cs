using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDroneProjectile : Projectile
{
    [SerializeField] protected MeleeAttackConfig shard;
    // [SerializeField] protected GameObject shard;

    // state
    protected Vector2 groundNormalVector;
    protected Vector3 groundFacingVector;
    protected Vector3 hitpoint;

    void Start() {
        groundNormalVector = this.transform.up;
        groundFacingVector = -Vector2.Perpendicular(groundNormalVector).normalized;
    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 1f, config.ObstacleLayerMask)) {
            groundNormalVector = hit.normal.normalized;
            groundFacingVector = -Vector2.Perpendicular(groundNormalVector).normalized;
            hitpoint = hit.point;
        }
    }

    protected override void HandleWallCollision(Collider entity) {
        shard.Attack(shard.AttackPrefab, hitpoint, groundFacingVector, true);

        base.HandleWallCollision(entity);
    }

    protected override void HandleTargetCollision(Collider entity) {
        // Debug.Log("DRONE PROJECTILE TARGET HIT");

        base.HandleTargetCollision(entity);
    }
}
