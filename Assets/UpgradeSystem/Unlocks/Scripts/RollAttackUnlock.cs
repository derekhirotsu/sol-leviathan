using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Unlocks/Roll Attack")]
public class RollAttackUnlock : Item {
    [Header("Roll Attack Unlock Fields")]

    [SerializeField]
    ScriptableVariables.BoolVariable rollAttackUnlocked;

    public override bool CheckAvailabilityRequirements() {
        // True if not already unlocked.
        return !rollAttackUnlocked.Value;
    }

    public override void Purchase() {
        rollAttackUnlocked.SetValue(true);
    }
}
