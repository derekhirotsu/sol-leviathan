using UnityEngine;
using ScriptableVariables;

public class BaseAttackConfig : ScriptableObject {
    [SerializeField]
    protected GameObject attackPrefab;
    public GameObject AttackPrefab { get { return attackPrefab; } }

    [SerializeField]
    protected LayerMask obstacleLayerMask;
    public LayerMask ObstacleLayerMask { get { return obstacleLayerMask; } }

    [SerializeField]
    protected LayerMask targetLayerMask;
    public LayerMask TargetLayerMask { get { return targetLayerMask; } }

    [SerializeField]
    protected ScriptableVariableReference<float> damage;
    public float Damage { get { return damage.Value; } }

    [SerializeField]
    protected DamageType damageType;
    public DamageType DamageType { get { return damageType; } }

    [SerializeField]
    protected ScriptableVariableReference<float> overrideHitRecoveryTime;
    public float OverrideHitRecoveryTime { get { return overrideHitRecoveryTime.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> forceImpulseX;

    [SerializeField]
    protected ScriptableVariableReference<float> forceImpulseY;

    public Vector3 ForceImpulse {
        get { return new Vector3(forceImpulseX, forceImpulseY, 0); }
    }

    [SerializeField]
    protected ScriptableVariableReference<float> timeToLive;
    public float TimeToLive { get { return timeToLive.Value; } }
}
