using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Unlocks/Air Melee")]
public class AirMeleeUnlock : Item {
    [Header("Air Melee Unlock Fields")]

    [SerializeField]
    ScriptableVariables.BoolVariable airMeleeUnlocked;

    public override bool CheckAvailabilityRequirements() {
        // True if not already unlocked.
        return !airMeleeUnlocked.Value;
    }

    public override void Purchase() {
        airMeleeUnlocked.SetValue(true);
    }
}
