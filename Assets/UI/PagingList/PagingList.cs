using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class PageChangeEvent : UnityEvent<int> {}

public class PagingList : MonoBehaviour {
    [SerializeField]
    List<GameObject> pageElements;

    [SerializeField]
    TMP_Text pageLabel;

    [SerializeField]
    CanvasGroup pageControls;

    [SerializeField]
    int maxElementsPerPage;

    [SerializeField]
    int maxElements;

    [SerializeField]
    bool showPageControls;

    [SerializeField]
    PageChangeEvent OnPageChange;

    int currentPageIndex;
    int totalPages {
        get {
            return Mathf.CeilToInt((float)maxElements / maxElementsPerPage);
        }
    }
    bool shouldShowControls {
        get { return pageElements.Count > maxElementsPerPage; }
    }

    void Start() {
        if (showPageControls && shouldShowControls) {
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

    public void GetPage(int index) {
        if (index < 0 || index >= totalPages - 1) {
            return;
        }

        for (int i = 0; i < pageElements.Count; i++) {
            int elementIndex = (index * maxElementsPerPage) + i;

            if (elementIndex >= maxElements) {
                pageElements[i].SetActive(false);
                continue;
            }

            pageElements[i].SetActive(true);
        }

        currentPageIndex = index;

        if (showPageControls && shouldShowControls) {
            pageLabel.text = $"<mspace=0.65em>{currentPageIndex + 1}/{totalPages}";
        }

        OnPageChange.Invoke(currentPageIndex);

        if (EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(pageElements[0]);
        } else if (!EventSystem.current.currentSelectedGameObject.activeSelf) {
            EventSystem.current.SetSelectedGameObject(pageElements[0]);
        } else {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);
        }
    }

    public void GetNextPage() {
        GetPage(currentPageIndex + 1);
    }
    
    public void GetPreviousPage() {
        GetPage(currentPageIndex - 1);
    }
}
