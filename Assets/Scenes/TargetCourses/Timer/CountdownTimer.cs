using UnityEngine;
using TMPro;

public class CountdownTimer: MonoBehaviour {
    [SerializeField]
    protected TMP_Text timeLabel;

    public float remainingTime = 1f;

    public string timeString;

    void Update() {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f) {
            remainingTime = 0f;
            enabled = false;
        }

        timeString = TimeFormat.FormatSeconds(remainingTime);
        timeLabel.text = timeString;
    }
}
