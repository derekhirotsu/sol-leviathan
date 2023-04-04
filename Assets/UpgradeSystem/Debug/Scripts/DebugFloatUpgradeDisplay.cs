using UnityEngine;
using UnityEngine.UI;

public class DebugFloatUpgradeDisplay : MonoBehaviour {
    [SerializeField]
    Text upgradeStatus;

    [SerializeField]
    ScriptableVariables.ScriptableVariableReference<float> floatUpgrade;

    void OnEnable() {
        UpdateUpgradeLabel(floatUpgrade);

        floatUpgrade.Subscribe(UpdateUpgradeLabel);
    }

    void OnDisable() {
        floatUpgrade.Unsubscribe(UpdateUpgradeLabel);
    }

    void UpdateUpgradeLabel(float value) {
        upgradeStatus.text = value.ToString();
    }
}
