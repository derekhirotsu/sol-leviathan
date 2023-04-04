using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public class Menu2 : MonoBehaviour {
    [SerializeField]
    string slug;
    public string Slug {
        get { return slug; }
    }

    [SerializeField]
    protected Selectable defaultSelection;

    [SerializeField]
    public UnityEvent OnShow;

    [SerializeField]
    public UnityEvent OnHide;

    [SerializeField]
    public UnityEvent OnSubmit;

    [SerializeField]
    public UnityEvent OnCancel;

    [SerializeField]
    public UnityEvent OnTabLeft;

    [SerializeField]
    public UnityEvent OnTabRight;

    [SerializeField]
    public UnityEvent OnClick;

    [SerializeField]
    public UnityEvent OnNavigate;

    protected Canvas canvas;
    protected CanvasGroup canvasGroup;

    public bool IsHidden {
        get {
            if (canvas == null) {
                return true;
            }
            
            return !canvas.enabled;
        }
    }

    protected virtual void Awake() {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (!canvas.enabled) {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;            
        }
    }

    public virtual void Show() {
        if (!IsHidden) {
            return;
        }

        canvas.enabled = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        OnShow.Invoke();
    }

    public virtual void Hide() {
        if (IsHidden) {
            return;
        }

        canvas.enabled = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        OnHide.Invoke();
    }

    public virtual void SelectElement() {
        if (defaultSelection != null) {
            defaultSelection.Select();
        }
    }

    public virtual void SetDefaultSelection(Selectable newElement) {
        defaultSelection = newElement;
    }

    public virtual void Submit(InputAction.CallbackContext context) {
        OnSubmit.Invoke();
    }

    public virtual void Cancel(InputAction.CallbackContext context) {
        OnCancel.Invoke();
    }

    public virtual void TabLeft(InputAction.CallbackContext context) {
        OnTabLeft.Invoke();
    }

    public virtual void TabRight(InputAction.CallbackContext context) {
        OnTabRight.Invoke();
    }

    public virtual void Click(InputAction.CallbackContext context) {
        OnClick.Invoke();
    }

    public virtual void Navigate(InputAction.CallbackContext context) {
        OnNavigate.Invoke();
    }
}
