using UnityEngine;

public class Melee : MonoBehaviour {
    protected bool configured = false;
    protected MeleeAttackConfig config;
    protected DamageSource damageSource;
    public void Configure(MeleeAttackConfig newConfig, DamageSource source) {
        config = newConfig;
        damageSource = source;
        Destroy(this.gameObject, config.TimeToLive);
        
        configured = true;
    }

    void FixedUpdate() {
        if (!configured) {
            return;
        }
    }

    void OnTriggerEnter(Collider entity) {
        if (!config.TargetLayerMask.Contains(entity.gameObject.layer)) {
            return;
        }

        var hitbox = entity.gameObject.GetComponent<HitDetection.Hitbox>();

        if (hitbox == null) {
            return;
        }

        Vector3 lineOfSightOrigin = damageSource.SourceEntity.transform.position;
        Vector3 rayDirection = (entity.transform.position - lineOfSightOrigin).normalized;
        float rayDistance = (entity.transform.position - lineOfSightOrigin).magnitude;

        if (CheckLineOfSightForWalls(lineOfSightOrigin, rayDirection, rayDistance)) {
            return;
        }

        if (CheckLineOfSightForOtherHitboxes(lineOfSightOrigin, rayDirection, rayDistance, entity)) {
            return;
        }

        if (hitbox.HandleHit(damageSource)) {
            OnHitCallback();
        }
    }

    // Check if line of sight is blocked by walls/platforms.
    bool CheckLineOfSightForWalls(Vector3 castOrigin, Vector3 castDirection, float castDistance) {
        return Physics.Raycast(castOrigin, castDirection, castDistance, config.ObstacleLayerMask);
    }

    // check if line of sight is blocked by other hitboxes
    bool CheckLineOfSightForOtherHitboxes(Vector3 castOrigin, Vector3 castDirection, float castDistance, Collider originalEntity) {
        Debug.DrawRay(castOrigin, castDirection * castDistance, Color.red, 3f);
        RaycastHit hit;
        if (Physics.SphereCast(castOrigin, 0.5f, castDirection, out hit, castDistance, config.TargetLayerMask)) {
            if (hit.collider != originalEntity) {
                var castHitbox = hit.collider.gameObject.GetComponent<HitDetection.Hitbox>();

                if (castHitbox == null) {
                    return false;
                }

                // return !castHitbox.IsIntangible(damageSource);
                return castHitbox.IsInvulnerable(damageSource);
            }
        }

        return false;
    }

    protected virtual void OnHitCallback() { }
}
