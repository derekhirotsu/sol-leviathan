using UnityEngine;
using UnityEngine.UI;

public class DebugIntUpgradeDisplay : MonoBehaviour {
    [SerializeField]
    Text upgradeStatus;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<int> intUpgrade;

    void OnEnable() {
        UpdateUpgradeStatus(intUpgrade);

        intUpgrade.Subscribe(UpdateUpgradeStatus);
    }

    void OnDisable() {
        intUpgrade.Unsubscribe(UpdateUpgradeStatus);
    }

    void UpdateUpgradeStatus(int value) {
        upgradeStatus.text = value.ToString();
    }
}
