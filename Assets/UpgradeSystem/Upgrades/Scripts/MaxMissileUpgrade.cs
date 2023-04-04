using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Upgrades/Maximum Missile Ammo")]
public class MaxMissileUpgrade : Item {
    [Header("Max Missile Upgrade Fields")]

    [SerializeField]
    ScriptableVariables.IntVariable missileCapacity;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> missilesUnlocked;

    [SerializeField]
    protected int missileCapacityUpperBound;

    [SerializeField]
    [Min(0)]
    protected int increaseAmount;

    public override bool CheckAvailabilityRequirements() {
        return missilesUnlocked && missileCapacity.Value < missileCapacityUpperBound;
    }

    public override void Purchase() {
        int newValue = Mathf.Clamp(missileCapacity.Value + increaseAmount, 1, missileCapacityUpperBound);

        missileCapacity.SetValue(newValue);
    }
}
