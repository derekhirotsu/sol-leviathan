using UnityEngine;

public class Projectile : MonoBehaviour {
    protected ProjectileAttackConfig config;

    public void SetStats(ProjectileAttackConfig newConfig) {
        config = newConfig;
        Destroy(this.gameObject, config.TimeToLive);
        
        configured = true;
    }
    protected bool configured = false;
    protected Vector3 direction;
    public void SetDirection(Vector3 newDirection) {
        direction = newDirection;
    }

    public virtual void FixedUpdate() {
        if (!configured) {
            return;
        }
        
        this.transform.position += (direction.normalized * config.ProjectileSpeed * Time.fixedDeltaTime);
    }

    protected void OnTriggerEnter(Collider entity) {
        // Projectile hit wall
        if (config.ObstacleLayerMask.Contains(entity.gameObject.layer)) {
            HandleWallCollision(entity);
        }

        // Projectile hit target
        if (config.TargetLayerMask.Contains(entity.gameObject.layer)) {
            HandleTargetCollision(entity);
        }
    }

    protected virtual void HandleWallCollision(Collider entity) {
        Destroy(this.gameObject);
    }

    protected virtual void HandleTargetCollision(Collider entity) {
        HitDetection.Hitbox hitbox = entity.gameObject.GetComponent<HitDetection.Hitbox>();

        if (hitbox == null) {
            return;
        }

        DamageSource damageSource = new DamageSource(
            this.gameObject,
            config.DamageType,
            config.Damage,
            config.ForceImpulse,
            config.OverrideHitRecoveryTime
        );

        if (hitbox.HandleHit(damageSource)) {
            OnHitCallback();
        }
    }

    protected virtual void OnHitCallback() {
        Destroy(this.gameObject);
    }
}
