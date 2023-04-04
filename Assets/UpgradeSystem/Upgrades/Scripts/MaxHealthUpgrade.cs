using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Upgrades/Maximum Health")]
public class MaxHealthUpgrade : Item {
    [Header("Max Health Upgrade Fields")]
    
    [SerializeField]
    ScriptableVariables.FloatVariable currentMaxHealth;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<float> maxHealthLimit;

    [SerializeField]
    [Min(0)]
    protected float increaseAmount;

    public override bool CheckAvailabilityRequirements() {
        return currentMaxHealth.Value < maxHealthLimit;
    }

    public override void Purchase() {
        float newValue = Mathf.Clamp(currentMaxHealth.Value + increaseAmount, 1, maxHealthLimit);

        currentMaxHealth.SetValue(newValue);
    }
}
