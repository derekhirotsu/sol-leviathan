using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using ScriptableVariables;

public class CourseSelectDisplay : MonoBehaviour {
    [SerializeField]
    TMP_Text pageLabel;

    [SerializeField]
    CourseDataList courses;

    [SerializeField]
    GameObject courseButtonDisplay;

    List<Button> courseButtons;
    List<TMP_Text> courseButtonsText;

    int currentPageIndex;
    int totalPages;

    [SerializeField]
    CourseInfoDisplay infoDisplay;
    void Start() {
        courseButtons = new List<Button>(courseButtonDisplay.GetComponentsInChildren<Button>());
        courseButtonsText = new List<TMP_Text>(courseButtons.Count);

        // cache text references
        foreach (var button in courseButtons) {
            var buttonText = button.gameObject.GetComponentInChildren<TMP_Text>();
            
            courseButtonsText.Add(buttonText);         
        }

        totalPages = Mathf.CeilToInt((float)courses.courseData.Count / 9);
        if (totalPages == 0) {
            totalPages = 1;
        }
    }

    public void SetupButtons(int pageIndex = 0) {
        for (int i = 0; i < courseButtons.Count; i++) {
            var button = courseButtons[i];
            var buttonText = courseButtonsText[i];
            
            int buttonIndex = (pageIndex * 9) + i;
            buttonText.text = (buttonIndex + 1).ToString("00");

            if (buttonIndex > courses.courseData.Count - 1) {
                buttonText.text = "---";
                button.interactable = false;
                button.targetGraphic.raycastTarget = false;
            } else {
                button.interactable = true;
                button.targetGraphic.raycastTarget = true;
            }
        }

        SelectCourseButton(0);

        pageLabel.text = $"{pageIndex + 1}/{totalPages}";
    }

    public void GetNextPage() {
        if (currentPageIndex + 1 == totalPages) {
            return;
        }

        currentPageIndex++;

        SetupButtons(currentPageIndex);
    }

    public void GetPreviousPage() {
        if (currentPageIndex == 0) {
            return;
        }

        currentPageIndex--;

        SetupButtons(currentPageIndex);
    }

    public void LoadCourse(int indexOffset) {
        int courseIndex = indexOffset + (currentPageIndex * 9);

        if (courseIndex > courses.courseData.Count - 1) {
            return;
        }

        SceneManager.LoadScene(courses.courseData[courseIndex].SceneName);
    }

    public void SelectCourse(int indexOffset) {
        int courseIndex = indexOffset + (currentPageIndex * 9);

        if (courseIndex > courses.courseData.Count - 1) {
            return;
        }

        infoDisplay.UpdateDisplay(courses.courseData[courseIndex]);
    }

    public void ResetCurrentPageIndex() {
        currentPageIndex = 0;
    }

    void SelectCourseButton(int index) {
        if (courseButtons == null) {
            return;
        }
        
        if (index < courseButtons.Count) {
            courseButtons[index].Select();
        }
    }
}
