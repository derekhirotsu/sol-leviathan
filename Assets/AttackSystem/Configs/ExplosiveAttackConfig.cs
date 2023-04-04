using UnityEngine;
using ScriptableVariables;

[CreateAssetMenu(menuName = "AttackConfig/ExplosiveAttack")]
public class ExplosiveAttackConfig : BaseAttackConfig { 
    [Header("Explosive Config")]

    [SerializeField]
    protected ScriptableVariableReference<float> explosionScale;
    public float ExplosionScale { get { return explosionScale.Value; } }

    public void Attack(GameObject source, Vector3 startPosition, bool parentedToSource = false) {
        DamageSource damageSource = new DamageSource(source, damageType, damage, ForceImpulse, overrideHitRecoveryTime);

        Explosive newExplosion;
        if (!parentedToSource) {
            newExplosion = Instantiate(attackPrefab, startPosition, Quaternion.identity).GetComponent<Explosive>();
        } else {
            newExplosion = Instantiate(attackPrefab, source.transform).GetComponent<Explosive>();
            newExplosion.transform.position += startPosition;
        }

        newExplosion.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);

        newExplosion.Configure(this, damageSource);
    }
}
