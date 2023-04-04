using UnityEngine;
using UnityEngine.UI;

public class DebugUnlockDisplay : MonoBehaviour {
    [SerializeField]
    Text unlockStatus;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<bool> unlockFlag;

    void OnEnable() {
        UpdateUnlockStatus(unlockFlag);

        unlockFlag.Subscribe(UpdateUnlockStatus);
    }

    void OnDisable() {
        unlockFlag.Unsubscribe(UpdateUnlockStatus);
    }

    string GetStatusText(bool status) {
        return status ? "Unlocked" : "Locked";
    }

    void UpdateUnlockStatus(bool status) {
        unlockStatus.text = GetStatusText(status);
    }
}
