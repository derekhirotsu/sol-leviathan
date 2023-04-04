using UnityEngine;
using TMPro;

public class FloatLabel : MonoBehaviour {
    [SerializeField]
    protected TMP_Text textDisplay;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<float> currentValue;

    void OnEnable() {
        SetText(currentValue.Value);
        currentValue.Subscribe(OnValueChange);
    }

    void OnDisable() {
        currentValue.Unsubscribe(OnValueChange);
    }

    void OnValueChange(float newValue) {
        SetText(newValue);
    }

    void SetText(float newValue) {
        textDisplay.text = newValue.ToString();
    }
}
