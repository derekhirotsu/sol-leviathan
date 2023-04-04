using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Unlocks/Ground Slam")]
public class GroundSlamUnlock : Item {
    [Header("Ground Slam Unlock Fields")]

    [SerializeField]
    ScriptableVariables.FloatVariable groundSlamScale;

    [SerializeField]
    ScriptableVariables.BoolVariable groundSlamUnlocked;

    [SerializeField]
    protected float initalGroundSlamScale;

    public override bool CheckAvailabilityRequirements() {
        return !groundSlamUnlocked.Value;
    }

    public override void Purchase() {
        groundSlamUnlocked.SetValue(true);
        groundSlamScale.SetValue(initalGroundSlamScale);
    }
}
