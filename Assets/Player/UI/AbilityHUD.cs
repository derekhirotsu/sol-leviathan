using UnityEngine;

public class AbilityHUD : Menu {
    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> abilityUnlocked;

    protected override void Awake() {
        base.Awake();
        CheckUnlockStatus(abilityUnlocked.Value);
    }

    void OnEnable() {
        abilityUnlocked.Subscribe(OnAbilityUnlockedChange);
    }

    void OnDisable() {
        abilityUnlocked.Unsubscribe(OnAbilityUnlockedChange);
    }

    void OnAbilityUnlockedChange(bool newValue) {
        CheckUnlockStatus(newValue);
    }

    void CheckUnlockStatus(bool value) {
        if (value) {
            ShowCanvas();
        } else {
            HideCanvas();
        }
    }
}
