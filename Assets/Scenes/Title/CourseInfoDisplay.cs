using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CourseInfoDisplay : MonoBehaviour {
    [SerializeField]
    TMP_Text courseName;

    [SerializeField]
    TMP_Text courseParTime;

    [SerializeField]
    TMP_Text courseBestTime;

    [SerializeField]
    GameObject LockedStatus;

    [SerializeField]
    TMP_Text UnlockRequirements;

    [SerializeField]
    GameObject UnlockedStatus;

    [SerializeField]
    TMP_Text UnlockedStatusHeading;

    [Header("Status Image")]
    [SerializeField]
    Image courseStatusImage;

    [SerializeField]
    Sprite unlockedSprite;

    [SerializeField]
    Sprite completedSprite;

    [SerializeField]
    Sprite parBeatSprite;

    public void UpdateDisplay(CourseData data) {
        courseName.text = data.DisplayName;

        if (data.CourseComplete.Value) {
            UnlockedStatusHeading.text = "CLEARED";
            float bestTimeInSeconds = (float)data.BestTime.Value / 1000;

            courseBestTime.text = "Best Time: " + TimeFormat.FormatSeconds(bestTimeInSeconds);
            if (bestTimeInSeconds <= data.ParTime) {
                courseStatusImage.sprite = parBeatSprite;
            } else {
                courseStatusImage.sprite = completedSprite;
            }
        } else {
            UnlockedStatusHeading.text = "UNCLEARED";
            courseBestTime.text = "Best Time: --:--:---";
            courseStatusImage.sprite = unlockedSprite;
        }

        courseParTime.text = "Par Time: " + TimeFormat.FormatSeconds(data.ParTime);
    }
}
