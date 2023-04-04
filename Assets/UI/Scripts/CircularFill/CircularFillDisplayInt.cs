using UnityEngine;

public class CircularFillDisplayInt : MonoBehaviour {
    [SerializeField]
    protected CircularFill circularFill;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> totalValue;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> currentValue;

    void OnEnable() {
        SetFillAmount();
        currentValue.Subscribe(OnValueChange);
        totalValue.Subscribe(OnValueChange);
    }

    void OnDisable() {
        currentValue.Unsubscribe(OnValueChange);
        totalValue.Unsubscribe(OnValueChange);
    }

    void OnValueChange(int newValue) {
        SetFillAmount();
    }

    void SetFillAmount() {
        float fillAmount = 1 - (float)currentValue / totalValue;

        circularFill.UpdateFillAmount(fillAmount);
    }

    // void Update() {
    //     float fillAmount = 1 - (float)currentValue / totalValue;

    //     circularFill.UpdateFillAmount(fillAmount);   
    // }
}
