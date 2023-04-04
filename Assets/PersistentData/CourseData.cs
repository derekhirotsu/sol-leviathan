using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableVariables;

public enum CourseRankStatus {
    NoRank,
    ClearedBronze,
    ClearedSilver,
    ClearedGold
}

[CreateAssetMenu(menuName = "CourseData")]
public class CourseData : ScriptableObject {
    public string Slug;
    public string DisplayName;
    public string SceneName;
    public BoolVariable CourseComplete;
    public IntVariable BestTime;
    public float ParTime;

    public bool _CourseComplete;
    public float _BestTime;
    public int Version;
    public float BronzeTime;
    public float SilverTime;
    public float GoldTime;

    public List<CourseData> requiredCourses;
    public bool RequriedCoursesCompleted {
        get { return requiredCourses.TrueForAll(course => course._CourseComplete); }
    }
    public int requiredCompletions = 0;

    void OnEnable() {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public CourseSaveData ToCourseSaveData() {
        return new CourseSaveData(CourseComplete.Value, BestTime.Value);
    }

    public void ApplySaveData(CourseSaveData saveData) {
        CourseComplete.SetValue(saveData.CourseComplete);
        BestTime.SetValue(saveData.BestTime);

        _CourseComplete = saveData.CourseComplete;
        _BestTime = saveData.BestTime;
    }

    public CourseRankStatus GetRankStatus() {
        if (_BestTime <= GoldTime) {
            return CourseRankStatus.ClearedGold;
        }

        if (_BestTime <= SilverTime) {
            return CourseRankStatus.ClearedSilver;
        }

        if (_BestTime <= BronzeTime) {
            return CourseRankStatus.ClearedBronze;
        }

        return CourseRankStatus.NoRank;
    }
}
