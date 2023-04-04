using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ScriptableVariables;

public class HowToPlayPages : MonoBehaviour, IMoveHandler  {
    [SerializeField]
    protected List<TopicInfo> topics;

    [SerializeField]
    protected TMP_Text PageHeading;

    [SerializeField]
    protected TMP_Text pageContent;

    [SerializeField]
    protected TMP_Text PageCount;

    [SerializeField]
    ScriptableVariableReference<bool> gamepadInputActive;

    int currentPageIndex;

    protected TopicInfo currentTopic;

    Selectable selectable;

    void OnEnable() {
        gamepadInputActive.Subscribe(OnGamepadInputActiveChange);
    }

    void OnDisable() {
        gamepadInputActive.Unsubscribe(OnGamepadInputActiveChange);
    }

    void Start() {
        selectable = GetComponent<Selectable>();
    }

    void OnGamepadInputActiveChange(bool gamepadActive) {
        if (currentTopic == null) {
            return;
        }

        SetPageInfo(currentTopic.Pages[currentPageIndex]);
    }

    void SetPageInfo(PageInfo page) {
        PageHeading.text = page.PageHeading;
        string controls = gamepadInputActive.Value ? page.PageGamepadControls : page.PageKeyboardControls;
        pageContent.text = controls + "\n\n" + page.PageDescription;
    }

    void SetPageCountText(int currentPage, int totalPages) {
        PageCount.text = $"{currentPage}/{totalPages}";
    }

    public void SetTopic(int index) {
        if (index < 0 || index > topics.Count) {
            return;
        }

        currentTopic = topics[index];
        currentPageIndex = 0;
        SetPageInfo(currentTopic.Pages[currentPageIndex]);
        SetPageCountText(currentPageIndex + 1, currentTopic.Pages.Count);
    }

    public void GetNextPage() {
        if (currentPageIndex + 1 == currentTopic.Pages.Count) {
            return;
        }

        currentPageIndex++;
        SetPageInfo(currentTopic.Pages[currentPageIndex]);
        SetPageCountText(currentPageIndex + 1, currentTopic.Pages.Count);
    }

    public void GetPreviousPage() {
        if (currentPageIndex <= 0) {
            return;
        }
        
        currentPageIndex--;
        SetPageInfo(currentTopic.Pages[currentPageIndex]);
        SetPageCountText(currentPageIndex + 1, currentTopic.Pages.Count);
    }

    //When the focus moves to another selectable object, Invoke this Method.
    public void OnMove(AxisEventData eventData) {
        //Assigns the move direction and the raw input vector representing the direction from the event data.
        MoveDirection moveDir = eventData.moveDir;
        Vector2 moveVector = eventData.moveVector;

        switch (moveDir) {
            case MoveDirection.Right:
                GetNextPage();
                break;
            case MoveDirection.Left:
                GetPreviousPage();
                break;
        }
    }
}
