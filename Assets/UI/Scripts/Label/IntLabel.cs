using UnityEngine;
using TMPro;

public class IntLabel : MonoBehaviour {
    [SerializeField]
    protected TMP_Text textDisplay;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> currentValue;

    void OnEnable() {
        SetText(currentValue.Value);
        currentValue.Subscribe(OnValueChange);
    }

    void OnDisable() {
        currentValue.Unsubscribe(OnValueChange);
    }

    void OnValueChange(int newValue) {
        SetText(newValue);
    }

    void SetText(int newValue) {
        textDisplay.text = newValue.ToString();
    }
}
