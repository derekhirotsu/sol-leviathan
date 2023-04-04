using UnityEngine;
using TMPro;
using ScriptableVariables;

public class Timer : MonoBehaviour {
    [SerializeField]
    protected TMP_Text timeLabel;

    public float elapsedTime = 0f;

    public string timeString;

    void Update() {
        // 59 minutes, 59 seconds, 999 milliseconds.
        if (elapsedTime >= 3599.999f) {
            elapsedTime = 3599.999f;
            return;
        }

        elapsedTime += Time.deltaTime;
        timeString = TimeFormat.FormatSecondsMonospace(elapsedTime, 0.65f);
        timeLabel.text = timeString;

    }
}
