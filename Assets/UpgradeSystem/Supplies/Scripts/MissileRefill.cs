using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Supplies/Missile Ammo")]
public class MissileRefill : Item {
    [Header("Missile Refill Fields")]

    [SerializeField]
    ScriptableVariables.IntVariable currentMissileAmmo;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<int> maxMissileAmmo;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> missilesUnlocked;

    [SerializeField]
    protected int ammoRefilled;

    public override bool CheckAvailabilityRequirements() {
        return missilesUnlocked && currentMissileAmmo.Value < maxMissileAmmo;
    }

    public override void Purchase() {
        int newValue = Mathf.Clamp(currentMissileAmmo.Value + ammoRefilled, 1, maxMissileAmmo);

        currentMissileAmmo.SetValue(newValue);
    }
}
