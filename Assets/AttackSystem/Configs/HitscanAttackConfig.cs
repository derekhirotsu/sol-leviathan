using UnityEngine;
using ScriptableVariables;

[CreateAssetMenu(menuName = "AttackConfig/HitscanAttack")]
public class HitscanAttackConfig : BaseAttackConfig {
    [Header("Hitscan Visuals Config")]
    [SerializeField]
    protected GameObject vfx_ImpactParticle;

    [Header("Hitscan Config")]

    [SerializeField]
    protected ScriptableVariableReference<float> fireRate;
    public float FireRate { get { return fireRate.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> raycastDistance;
    public float RaycastDistance { get { return raycastDistance.Value; } }

    public void Attack(GameObject source, Vector3 origin, Vector3 direction) {
        bool didHit = false;
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, raycastDistance, obstacleLayerMask | targetLayerMask)) {
            didHit = true;
            HitDetection.Hitbox hitbox = hit.collider.GetComponent<HitDetection.Hitbox>();

            if (hitbox != null) {
                DamageSource damageSource = new DamageSource(source, damageType, damage, ForceImpulse);
                didHit = hitbox.HandleHit(damageSource);
            }
        }

        Vector3 hitscanDestination = didHit ? hit.point : origin + (direction * RaycastDistance);
        Hitscan hitscanVisual = Instantiate(attackPrefab, origin, Quaternion.identity).GetComponent<Hitscan>();

        if (didHit && vfx_ImpactParticle != null) {
            GameObject impactEffect = Instantiate(vfx_ImpactParticle, hit.point, Quaternion.Euler(direction));
            Destroy(impactEffect, 0.7f);
        }

        hitscanVisual.SetDirection(origin);
        hitscanVisual.SetDestination(hitscanDestination);
        hitscanVisual.SetStats(this);
    }
}
