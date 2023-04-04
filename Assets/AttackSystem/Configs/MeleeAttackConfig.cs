using UnityEngine;
using ScriptableVariables;

[CreateAssetMenu(menuName = "AttackConfig/MeleeAttack")]
public class MeleeAttackConfig : BaseAttackConfig {
    [Header("Melee Config")]
    [SerializeField]
    protected GameObject meleeVFXPrefab;

    [SerializeField]
    protected ScriptableVariableReference<float> windupTime;
    public float WindupTime { get { return windupTime.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> swingTime;
    public float SwingTime { get { return swingTime.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<float> recoveryTime;
    public float RecoveryTime { get { return recoveryTime.Value; } }

    [SerializeField]
    protected ScriptableVariableReference<int> moveSpeedWhileStriking;
    public int MoveSpeedWhileStriking { get { return moveSpeedWhileStriking.Value; } }

    // [SerializeField]
    // protected ScriptableVariableReference<float> horizontalScale;
    // public float HorizontalScale { get { return horizontalScale.Value; } }

    // [SerializeField]
    // protected ScriptableVariableReference<float> verticalScale;
    // public float VerticalScale { get { return verticalScale.Value; } }

    [SerializeField]
    protected Vector3 directionOffset;
    
    [SerializeField]
    protected Vector3 positionOffset;

    public void Attack(GameObject source, Vector3 startPosition, Vector3 direction, bool parentedToSource = false) {
        DamageSource damageSource = new DamageSource(source, damageType, damage, ForceImpulse, overrideHitRecoveryTime);

        Melee newStrike;
        if (!parentedToSource) {
            newStrike = Instantiate(attackPrefab, startPosition + positionOffset, Quaternion.identity).GetComponent<Melee>();
        } else {
            newStrike = Instantiate(attackPrefab, source.transform).GetComponent<Melee>();
            newStrike.transform.position += startPosition;
        }
        
        
        Quaternion rotation = Quaternion.LookRotation(direction + directionOffset, Vector3.up);
        
        newStrike.transform.rotation = rotation;
        // newStrike.transform.localScale = new Vector3(1f, verticalScale, horizontalScale);

        newStrike.Configure(this, damageSource);
    }
}
