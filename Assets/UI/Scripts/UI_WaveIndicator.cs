using UnityEngine;
using TMPro;

public class UI_WaveIndicator : MonoBehaviour
{
    [SerializeField] protected TMP_Text waveText;

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<int> _waveNumber;

    [SerializeField] protected string waveMessage = "Wave ";

    void OnEnable() {
        SetWaveText(waveMessage + _waveNumber.Value);
        _waveNumber.Subscribe(OnWaveNumberChange);
    }

    void OnDisable() {
        _waveNumber.Unsubscribe(OnWaveNumberChange);
    }

    void OnWaveNumberChange(int newValue) {
        string newText = waveMessage + newValue;

        SetWaveText(newText);
    }

    void SetWaveText(string text) {
        waveText.text = text;
    }
}
