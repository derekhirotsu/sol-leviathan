using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TabChangeEvent : UnityEvent<int> {}

public class TabController : MonoBehaviour {
    [SerializeField]
    protected TabChangeEvent OnTabChange;

    protected List<Tab> tabs;
    protected int currentTabIndex;

    protected virtual void Start() {
        tabs = new List<Tab>(GetComponentsInChildren<Tab>());
    }

    public virtual void SetTab(int index) {
        if (index < 0 || index >= tabs.Count) {
            return;
        }

        tabs[currentTabIndex].DeselectTab();
        currentTabIndex = index;
        tabs[currentTabIndex].SelectTab();
        OnTabChange.Invoke(currentTabIndex);
    }

    public virtual void GetNextTab() {
        SetTab(currentTabIndex + 1);
    }

    public virtual void GetPreviousTab() {
        SetTab(currentTabIndex - 1);
    }
}
