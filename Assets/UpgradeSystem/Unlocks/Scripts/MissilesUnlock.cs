using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Unlocks/Missiles")]
public class MissilesUnlock : Item {
    [Header("Missiles Unlock Fields")]

    [SerializeField]
    ScriptableVariables.IntVariable maxMissiles;

    [SerializeField]
    ScriptableVariables.BoolVariable missilesUnlocked;

    [SerializeField]
    protected int initialMaxMissileValue;

    public override bool CheckAvailabilityRequirements() {
        return !missilesUnlocked.Value;
    }

    public override void Purchase() {
        missilesUnlocked.SetValue(true);
        maxMissiles.SetValue(initialMaxMissileValue);
    }
}
