using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CourseDisplay : MonoBehaviour {
    [SerializeField]
    GameObject courseButtonDisplay;

    [SerializeField]
    CanvasGroup pageControls;

    [SerializeField]
    TMP_Text pageLabel;

    [SerializeField]
    CourseInfo courseInfo;

    List<Button> buttons;
    public List<CourseData> courses;
    public List<CourseButton> courseButtons;

    int currentPageIndex;
    int totalPages;
    bool showPageControls;
    int currentButtonIndex;

    void Start() {
        buttons = new List<Button>(courseButtonDisplay.GetComponentsInChildren<Button>());
        courseButtons = new List<CourseButton>(courseButtons.Count);

        // cache text references
        foreach (var button in buttons) {
            var buttonText = button.gameObject.GetComponent<CourseButton>();
            
            courseButtons.Add(buttonText);         
        }
    }

    public void ResetPage() {
        currentPageIndex = 0;
    }

    public void SetCourses(List<CourseData> courseList) {
        courses = courseList;
        
        totalPages = Mathf.CeilToInt((float)courses.Count / 7);
        showPageControls = courses.Count > 7;
        if (showPageControls) {
            pageControls.alpha = 1f;
            pageControls.interactable = true;
            pageControls.blocksRaycasts = true;
            pageLabel.text = $"<mspace=0.65em>{currentPageIndex + 1}/{totalPages}";
        } else {
            pageControls.alpha = 0f;
            pageControls.interactable = false;
            pageControls.blocksRaycasts = false;
        }
    }

    public void OnCourseSelect(int index) {
        currentButtonIndex = index;
        int courseIndex = index + (currentPageIndex * 7);

        if (courseIndex > courses.Count - 1) {
            return;
        }

        var courseData = courses[courseIndex];
        courseInfo.SelectCourse(courseData);
    }

    public void GetPage(int pageIndex = 0) {
        for (int i = 0; i < courseButtons.Count; i++) {
            var button = courseButtons[i];
            int buttonIndex = (pageIndex * 7) + i;

            if (buttonIndex > courses.Count - 1) {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            button.courseName.text = courses[buttonIndex].DisplayName;
            button.courseNumber.text = (buttonIndex + 1).ToString("00");
        }

        if (showPageControls) {
            pageLabel.text = $"<mspace=0.65em>{currentPageIndex + 1}/{totalPages}";
        }
    }

    public void GetNextPage() {
        if (currentPageIndex + 1 == totalPages) {
            return;
        }

        currentPageIndex++;

        GetPage(currentPageIndex);

        if (EventSystem.current.currentSelectedGameObject == null) {
            buttons[0].Select();
        } else if (!EventSystem.current.currentSelectedGameObject.activeSelf) {
            buttons[0].Select();
        } else {
            int courseIndex = currentButtonIndex + (currentPageIndex * 7);
            var courseData = courses[courseIndex];
            courseInfo.SelectCourse(courseData);            
        }
    }

    public void GetPreviousPage() {
        if (currentPageIndex == 0) {
            return;
        }

        currentPageIndex--;

        GetPage(currentPageIndex);

        if (EventSystem.current.currentSelectedGameObject == null) {
            buttons[0].Select();
        } else if (!EventSystem.current.currentSelectedGameObject.activeSelf) {
            buttons[0].Select();
        } else {
            int courseIndex = currentButtonIndex + (currentPageIndex * 7);
            var courseData = courses[courseIndex];
            courseInfo.SelectCourse(courseData);            
        }
    }

    public void LoadCourse(int indexOffset) {
        int courseIndex = indexOffset + (currentPageIndex * 9);

        if (courseIndex > courses.Count - 1) {
            return;
        }

        SceneManager.LoadScene(courses[courseIndex].SceneName);
    }
}
