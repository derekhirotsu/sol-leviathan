using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Upgrades/Ground Slam Radius")]
public class GroundSlamRadiusUpgrade : Item {
    [Header("Ground Slam Radius Upgrade Fields")]

    [SerializeField]
    ScriptableVariables.FloatVariable currentGroundSlamRadius;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<float> groundSlamRadiusLimit;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> groundSlamUnlocked;

    // [SerializeField]
    // AttackDataConfigThing groundSlamConfigThing;

    [SerializeField]
    [Min(0)]
    protected float increaseAmount;

    public override bool CheckAvailabilityRequirements() {
        bool valueWithinBounds = currentGroundSlamRadius.Value < groundSlamRadiusLimit;

        return groundSlamUnlocked && valueWithinBounds;
    }

    public override void Purchase() {
        float newValue = Mathf.Clamp(currentGroundSlamRadius.Value + increaseAmount, 1, groundSlamRadiusLimit);

        // groundSlamConfigThing.BaseDamage = 123;

        currentGroundSlamRadius.SetValue(newValue);
    }
}
