using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Unlocks/Air Shot")]
public class AirShotUnlock : Item {
    [Header("Air Shot Unlock Fields")]

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> hoverUnlocked;

    [SerializeField]
    ScriptableVariables.BoolVariable airShotUnlocked;

    public override bool CheckAvailabilityRequirements() {
        // if hover has been unlocked and air shot has not yet been unlocked.
        return hoverUnlocked.Value && !airShotUnlocked.Value;
    }

    public override void Purchase() {
        airShotUnlocked.SetValue(true);
    }
}
