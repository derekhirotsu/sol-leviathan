using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Upgrades/Hover Time")]
public class MaxHoverTime : Item {
    [Header("Hover Time Upgrade Fields")]

    [SerializeField]
    ScriptableVariables.FloatVariable maxHoverTime;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> hoverUnlocked;

    [SerializeField]
    protected float maxHoverTimeUpperBound;

    [SerializeField]
    [Min(0)]
    protected float increaseAmount;

    public override bool CheckAvailabilityRequirements() {
        return hoverUnlocked && maxHoverTime.Value < maxHoverTimeUpperBound;
    }

    public override void Purchase() {
        float newValue = Mathf.Clamp(maxHoverTime.Value + increaseAmount, 1, maxHoverTimeUpperBound);

        maxHoverTime.SetValue(newValue);
    }
}
