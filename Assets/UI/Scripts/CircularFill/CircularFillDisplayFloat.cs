using UnityEngine;

public class CircularFillDisplayFloat : MonoBehaviour {
    [SerializeField]
    protected CircularFill circularFill;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<float> totalValue;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<float> currentValue;

    void OnEnable() {
        SetFillAmount();
        currentValue.Subscribe(OnValueChange);
        totalValue.Subscribe(OnValueChange);
    }

    void OnDisable() {
        currentValue.Unsubscribe(OnValueChange);
        totalValue.Unsubscribe(OnValueChange);
    }

    void OnValueChange(float newValue) {
        SetFillAmount();
    }

    void SetFillAmount() {
        float fillAmount = 1 - currentValue / totalValue;

        circularFill.UpdateFillAmount(fillAmount);       
    }
}
