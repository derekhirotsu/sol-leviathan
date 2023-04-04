using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Unlocks/Hover")]
public class HoverUnlock : Item {
    [Header("Hover Unlock Fields")]

    [SerializeField]
    ScriptableVariables.FloatVariable maxHoverTime;

    [SerializeField]
    ScriptableVariables.BoolVariable hoverUnlocked;

    [SerializeField]
    protected float initialStabilizerTime;

    public override bool CheckAvailabilityRequirements() {
        // if hover has not yet been unlocked.
        return !hoverUnlocked.Value;
    }

    public override void Purchase() {
        hoverUnlocked.SetValue(true);
        maxHoverTime.SetValue(initialStabilizerTime);
    }
}
