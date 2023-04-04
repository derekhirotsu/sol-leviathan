using UnityEngine;
using ScriptableVariables;

[CreateAssetMenu(menuName = "AttackConfig/ProjectileAttack")]
public class ProjectileAttackConfig : BaseAttackConfig {
    [Header("Projectile Config")]

    [SerializeField]
    protected ScriptableVariableReference<float> fireRate;
    public float FireRate { get { return fireRate.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> projectileSpeed;
    public float ProjectileSpeed { get { return projectileSpeed.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> projectileScale;
    public float ProjectileScale { get { return projectileScale.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> deviation;
    public float Deviation { get { return deviation.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<int> numberOfProjectiles;
    public int NumberOfProjectiles { get { return numberOfProjectiles.Value; } }

    public void Attack(Vector3 origin, Vector3 direction, Quaternion rotation) {
        float spreadDeviation = -deviation/2; // The starting deviation that is increased.
        float iterations = deviation / numberOfProjectiles; // The amount to increase the spread by each iteration.

        for (int i = 0; i < numberOfProjectiles; ++i) {
            Vector3 aimVector = FieldOfView.DirFromAngleXY(spreadDeviation, direction.normalized);

            Projectile newProjectile = Instantiate(attackPrefab, origin, rotation).GetComponent<Projectile>();
            
            newProjectile.transform.localScale = new Vector3 (projectileScale, projectileScale, projectileScale);
            newProjectile.SetDirection(aimVector);
            newProjectile.SetStats(this);

            spreadDeviation += iterations;

        }
    }
}
