using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CourseInfo : MonoBehaviour {
    [SerializeField]
    TMP_Text courseName;

    [SerializeField]
    TMP_Text courseBestTime;

    [SerializeField]
    Image courseBestRankIcon;

    [SerializeField]
    TMP_Text courseGoldTime;

    [SerializeField]
    TMP_Text courseSilverTime;

    [SerializeField]
    TMP_Text courseBronzeTime;

    [SerializeField]
    Image goldRankIcon;

    [SerializeField]
    Image silverRankIcon;

    [SerializeField]
    Image bronzeRankIcon;

    [SerializeField]
    Sprite noRankSprite;

    [SerializeField]
    Sprite unclearedSprite;

    public void SelectCourse(CourseData data) {
        courseName.text = data.DisplayName;

        // personal records
        GetRankSprite(data);
        GetBestTime(data);

        // Rank information
        courseGoldTime.text = TimeFormat.FormatSecondsMonospace(data.GoldTime);
        courseSilverTime.text = TimeFormat.FormatSecondsMonospace(data.SilverTime);
        courseBronzeTime.text = TimeFormat.FormatSecondsMonospace(data.BronzeTime);
    }

    void GetRankSprite(CourseData data) {
        CourseRankStatus status = data.GetRankStatus();

        if (!data._CourseComplete) {
            courseBestRankIcon.sprite = unclearedSprite;
            courseBestRankIcon.color = Color.gray;
            return;         
        }

        if (status == CourseRankStatus.ClearedGold) {
            courseBestRankIcon.color = goldRankIcon.color;
            courseBestRankIcon.sprite = goldRankIcon.sprite;
            return;
        }

        if (status == CourseRankStatus.ClearedSilver) {
            courseBestRankIcon.color = silverRankIcon.color;
            courseBestRankIcon.sprite = silverRankIcon.sprite;
            return;
        }

        if (status == CourseRankStatus.ClearedBronze) {
            courseBestRankIcon.color = bronzeRankIcon.color;
            courseBestRankIcon.sprite = bronzeRankIcon.sprite;
            return;
        }

        if (status == CourseRankStatus.NoRank) {
            courseBestRankIcon.sprite = noRankSprite;
            courseBestRankIcon.color = Color.white;
        }
    }

    void GetBestTime(CourseData data) {
        if (data._CourseComplete) {   
            courseBestTime.text = TimeFormat.FormatSecondsMonospace(data._BestTime);
        } else {
            courseBestTime.text = "Uncleared";
        }
    }
}
