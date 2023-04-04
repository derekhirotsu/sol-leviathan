using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CourseSaveData {
    public bool CourseComplete;
    public int BestTime;

    public CourseSaveData() {}

    public CourseSaveData(bool courseComplete, int bestTime) {
        CourseComplete = courseComplete;
        BestTime = bestTime;
    }
}

