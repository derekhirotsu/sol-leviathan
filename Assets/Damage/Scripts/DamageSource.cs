using UnityEngine;

public struct DamageSource {
    private GameObject sourceEntity;
    public GameObject SourceEntity { get { return sourceEntity; } }

    private DamageType damageType;
    public DamageType DamageType { get { return damageType; } }

    private float rawDamageValue;
    public float RawDamageValue { get { return rawDamageValue; } }

    private Vector3 forceImpulse;
    public Vector3 ForceImpulse { get { return forceImpulse; } }

    private float overrideHitRecoveryTime;
    public float OverrideHitRecoveryTime { get { return overrideHitRecoveryTime; } }
    public bool ShouldOverrideHitRecovery { get { return overrideHitRecoveryTime > 0f; } }

    public DamageSource(GameObject source, DamageType type, float value, Vector3 force, float recoveryTime = 0f) {
        sourceEntity = source;
        damageType = type;
        rawDamageValue = value;
        forceImpulse = force;
        overrideHitRecoveryTime = recoveryTime;
    }

    public override string ToString() {
        return $"{sourceEntity.name} attacking for {rawDamageValue} damage of type {damageType} with force impulse vector: {forceImpulse}";
    }
}
