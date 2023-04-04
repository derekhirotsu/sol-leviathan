using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseTime : MonoBehaviour {
    TMP_Text timeDisplay;
    TargetCourseController courseController;

    void Start() {
        courseController = GetComponentInParent<TargetCourseController>();
        timeDisplay = GetComponent<TMP_Text>();
        timeDisplay.text = TimeFormat.FormatSecondsMonospace(0, 0.65f);
    }

    public void SyncTimeDisplay() {
        if (courseController == null || timeDisplay == null) {
            return;
        }

        timeDisplay.text = courseController.FormattedElapsedTime;
    }

    // string Sass() {
    //     string[] remarks = {
    //         "Maybe try another level.",
    //         "Has it really been an hour?",
    //         "Yeah, the timer caps out at an hour.",
    //         "You sure are persistent.",
    //         "No, there isn't an easter egg for taking this long.",
    //         "Wake me up when you decide to finish the level."
    //     };
        
    //     string remark = remarks[Random.Range(0, remarks.Length)];

    //     return remark;
    // }
}
