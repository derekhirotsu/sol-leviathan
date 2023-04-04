using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CourseSetDisplay : MonoBehaviour {
    [SerializeField]
    List<CourseSet> courseSets;

    [SerializeField]
    CourseDisplay courseDisplay;

    [SerializeField]
    GameObject courseButtonsRoot;

    [SerializeField]
    GameObject unlockedDisplay;

    [SerializeField]
    GameObject lockedDisplay;

    [SerializeField]
    TMP_Text SetNameDisplay;

    [SerializeField]
    TMP_Text CourseCompletedDisplay;

    [SerializeField]
    TMP_Text lockedMessage;

    [SerializeField]
    TMP_Text pageLabel;

    [SerializeField]
    CanvasGroup setPageControls;

    MenuController2 menuController;

    Button[] courseSetButtons;
    List<TMP_Text> courseButtonsText;

    int currentPageIndex;
    int totalPages;
    bool showPageControls;
    int currentButtonIndex;

    void Start() {
        courseSetButtons = courseButtonsRoot.GetComponentsInChildren<Button>();
        courseButtonsText = new List<TMP_Text>(courseSetButtons.Length);
        menuController = GetComponentInParent<MenuController2>();

        // cache text references
        foreach (var button in courseSetButtons) {
            var buttonText = button.gameObject.GetComponentInChildren<TMP_Text>();
            
            courseButtonsText.Add(buttonText);         
        }

        totalPages = Mathf.CeilToInt((float)courseSets.Count / 7);
        showPageControls = courseSets.Count > 7;
        if (showPageControls) {
            setPageControls.alpha = 1f;
            setPageControls.interactable = true;
            setPageControls.blocksRaycasts = true;
            pageLabel.text = $"<mspace=0.65em>{currentPageIndex + 1}/{totalPages}";
        } else {
            setPageControls.alpha = 0f;
            setPageControls.interactable = false;
            setPageControls.blocksRaycasts = false;
        }
    }

    public void OnSetClick(int indexOffset) {
        int courseIndex = indexOffset + (currentPageIndex * 7);

        if (courseIndex > courseSets.Count - 1) {
            return;
        }

        if (!courseSets[courseIndex].SetUnlocked) {
            return;
        }

        menuController.ShowMenu("set_courses");
        courseDisplay.SetCourses(courseSets[courseIndex].courses);
        courseDisplay.GetPage(0);
    }

    public void OnSetSelect(int index) {
        currentButtonIndex = index;
        int courseIndex = index + (currentPageIndex * 7);

        if (courseIndex > courseSets.Count - 1) {
            return;
        }

        var set = courseSets[courseIndex];
        UpdateSetInfo(set);
    }

    public void UpdateSetInfo(CourseSet set) {
        SetNameDisplay.text = set.SetName;

        if (set.SetUnlocked) {
            int completed = 0;
            foreach (var course in set.courses) {
                if (course._CourseComplete) {
                    completed++;
                }
            }

            CourseCompletedDisplay.text = string.Format("<mspace=0.65em>{0:00}/{1:00}", completed, set.courses.Count);

            unlockedDisplay.SetActive(true);
            lockedDisplay.SetActive(false);
        } else {

            lockedMessage.text = set.lockedMessage;
            unlockedDisplay.SetActive(false);
            lockedDisplay.SetActive(true);
        }
    }

    public void ResetPage() {
        currentPageIndex = 0;
    }

    public void GetPage(int pageIndex = 0) {
        for (int i = 0; i < courseSetButtons.Length; i++) {
            var button = courseSetButtons[i];
            var buttonText = courseButtonsText[i];
            int buttonIndex = (pageIndex * 7) + i;

            if (buttonIndex >= courseSets.Count) {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            buttonText.text = courseSets[buttonIndex].SetName;
        }

        if (showPageControls){
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
            courseSetButtons[0].Select();
        } else if (!EventSystem.current.currentSelectedGameObject.activeSelf) {
            courseSetButtons[0].Select();
        } else {
            int courseIndex = currentButtonIndex + (currentPageIndex * 7);
            var set = courseSets[courseIndex];
            UpdateSetInfo(set);
        }
    }

    public void GetPreviousPage() {
        if (currentPageIndex == 0) {
            return;
        }

        currentPageIndex--;
        GetPage(currentPageIndex);

        if (EventSystem.current.currentSelectedGameObject == null) {
            courseSetButtons[0].Select();
        } else if (!EventSystem.current.currentSelectedGameObject.activeSelf) {
            courseSetButtons[0].Select();
        } else {
            int courseIndex = currentButtonIndex + (currentPageIndex * 7);
            var set = courseSets[courseIndex];
            UpdateSetInfo(set);
        }
    }
}
