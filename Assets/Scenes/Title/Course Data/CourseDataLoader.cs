using System.Collections.Generic;
using UnityEngine;

public class CourseDataLoader : MonoBehaviour {
    [SerializeField]
    CourseDataList courses;

    static bool CourseDataInitialized;

    void Start() {
        InitializeCourseData();
    }

    void InitializeCourseData() {
        if (CourseDataInitialized) {
            return;
        }

        if (!PersistentData.TryValidateCourseSaveDirectory()) {
            return;
        }

        CourseSaveData defaultData = new CourseSaveData();
        CourseSaveData saveData;

        foreach (var course in courses.courseData) {
            saveData = PersistentData.TryLoadCourseData(course.Slug);

            // an issue occurred with loading data for this course.
            if (saveData == null) {
                course.ApplySaveData(defaultData);
                break;
            }

            course.ApplySaveData(saveData);
        }

        CourseDataInitialized = true;
    }
}
