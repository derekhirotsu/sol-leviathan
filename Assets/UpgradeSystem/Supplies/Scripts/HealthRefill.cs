using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Supplies/Health")]
public class HealthRefill : Item {
    [Header("Health Refill Fields")]

    [SerializeField]
    ScriptableVariables.FloatVariable currentHealth;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<float> currentMaxHealth;

    [SerializeField]
    protected float healthRefilled;

    public override bool CheckAvailabilityRequirements() {
        return currentHealth.Value < currentMaxHealth;
    }

    public override void Purchase() {
        float newValue = Mathf.Clamp(currentHealth.Value + healthRefilled, 1, currentMaxHealth);

        currentHealth.SetValue(newValue);
    }
}
