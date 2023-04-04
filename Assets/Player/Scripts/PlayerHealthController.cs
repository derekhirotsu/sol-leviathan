using UnityEngine;

public class PlayerHealthController : HitDetection.HealthController {
    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<float> maxHealthModifier;

    public override float MaxHealth {
        get { return profile.BaseMaxHealth + maxHealthModifier; }
    }

    protected override void OnEnable() {
        base.OnEnable();
        maxHealthModifier.Subscribe(OnMaxHealthModifierChange);
    }

    protected override void OnDisable() {
        base.OnDisable();
        maxHealthModifier.Unsubscribe(OnMaxHealthModifierChange);
    }

    protected void OnMaxHealthModifierChange(float newValue) {
        SetCurrentHealth(newValue);
    }
}
