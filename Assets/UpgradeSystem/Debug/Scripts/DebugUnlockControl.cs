using UnityEngine;
using UnityEngine.UI;

public class DebugUnlockControl : MonoBehaviour {
    [SerializeField]
    Button button;

    [SerializeField]
    Text buttonLabel;

    [SerializeField]
    ScriptableVariables.BoolVariable unlockValue;
    
    void OnEnable() {
        UpdateButtonText(unlockValue.Value);

        button.onClick.AddListener(ToggleUnlock);
    }

    void OnDisable() {
        button.onClick.RemoveListener(ToggleUnlock);
    }

    void ToggleUnlock() {
        unlockValue.ToggleChange();

        UpdateButtonText(unlockValue.Value);
    }

    void UpdateButtonText(bool status) {
        buttonLabel.text = status ? "Lock" : "Unlock";
    }
}
