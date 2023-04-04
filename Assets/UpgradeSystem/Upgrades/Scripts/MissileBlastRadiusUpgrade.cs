using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Upgrades/Missile Blast Radius")]
public class MissileBlastRadiusUpgrade : Item {
    [Header("Missile Blast Radius Upgrade Fields")]

    [SerializeField]
    ScriptableVariables.FloatVariable currentMissileBlastRadius;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<float> missileBlastRadiusLimit;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> missilesUnlocked;

    [SerializeField]
    [Min(0)]
    protected float increaseAmount;

    public override bool CheckAvailabilityRequirements() {
        bool valueWithinBounds = currentMissileBlastRadius.Value < missileBlastRadiusLimit;

        return missilesUnlocked && valueWithinBounds;
    }

    public override void Purchase() {
        float newValue = Mathf.Clamp(currentMissileBlastRadius.Value + increaseAmount, 1, missileBlastRadiusLimit);

        currentMissileBlastRadius.SetValue(newValue);
    }
}
