using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HowToPlayTopicDisplay : MonoBehaviour {
    [SerializeField]
    MenuController2 menuController;

    [SerializeField]
    List<HowToPlayInfo> infoTopics;

    [SerializeField]
    GameObject buttonsRoot;

    [SerializeField]
    CanvasGroup setPageControls;

    [SerializeField]
    TMP_Text pageLabel;

    Button[] buttons;
    List<TMP_Text> buttonsText;

    int currentPageIndex;
    int totalPages;
    bool showPageControls;
    int currentButtonIndex;

    void Start() {
        buttons = buttonsRoot.GetComponentsInChildren<Button>();
        buttonsText = new List<TMP_Text>(buttons.Length);

        // cache text references
        foreach (var button in buttons) {
            var buttonText = button.gameObject.GetComponentInChildren<TMP_Text>();
            
            buttonsText.Add(buttonText);         
        }

        totalPages = Mathf.CeilToInt((float)infoTopics.Count / 7);
        showPageControls = infoTopics.Count > 7;
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

    public void OnTopicClick(int indexOffset) {
        int topicIndex = indexOffset + (currentPageIndex * 7);

        if (topicIndex > infoTopics.Count - 1) {
            return;
        }

        menuController.ShowMenu(infoTopics[topicIndex].MenuName);
    }

    public void ResetPage() {
        currentPageIndex = 0;
    }

    public void GetPage(int pageIndex) {
        for (int i = 0; i < buttons.Length; i++) {
            var button = buttons[i];
            var buttonText = buttonsText[i];
            int buttonIndex = (pageIndex * 7) + i;

            if (buttonIndex >= infoTopics.Count) {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            buttonText.text = infoTopics[buttonIndex].TopicName;
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
            buttons[0].Select();
        } else if (!EventSystem.current.currentSelectedGameObject.activeSelf) {
            buttons[0].Select();
        } else {
            int topicIndex = currentButtonIndex + (currentPageIndex * 7);
            var set = infoTopics[topicIndex];
            menuController.ShowMenu(infoTopics[topicIndex].MenuName);
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
            int topicIndex = currentButtonIndex + (currentPageIndex * 7);
            var set = infoTopics[topicIndex];
            menuController.ShowMenu(infoTopics[topicIndex].MenuName);
        }
    }
}
