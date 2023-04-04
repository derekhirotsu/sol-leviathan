using UnityEngine;

public class Missile : Projectile {
    [SerializeField]
    protected GameObject missileExplosionEffect;

    [SerializeField]
    protected ExplosiveAttackConfig missileExplosionConfig;

    protected override void HandleWallCollision(Collider entity) {
        HitEffect();
        base.HandleWallCollision(entity);
    }

    protected override void HandleTargetCollision(Collider entity) {
        HitEffect();
        base.HandleTargetCollision(entity);
    }

    protected void HitEffect() {
        GameObject explosion = Instantiate(missileExplosionEffect, this.transform.position, Quaternion.identity);

        Destroy(explosion, missileExplosionConfig.TimeToLive);

        missileExplosionConfig.Attack(explosion, explosion.transform.position);
    }
}
