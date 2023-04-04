using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public class Menu : MonoBehaviour {
    [SerializeField]
    public string Slug;

    // [SerializeField]
    // protected Button defaultSelectedButton;

    [SerializeField]
    protected Selectable defaultSelection;

    [SerializeField]
    public UnityEvent OnShowMenu;

    [SerializeField]
    public UnityEvent OnHideMenu;

    // Decprecated; Rely on automatically finding Selectables in menu;
    // [SerializeField]
    // protected List<Button> buttons;

    // public Selectable[] selectables;
    protected Canvas canvas;
    protected CanvasGroup canvasGroup;

    public virtual bool IsHidden {
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

        enabled = canvas != null;

        // selectables = GetComponentsInChildren<Selectable>();
    }

    public virtual void ShowCanvas() {
        if (canvas != null) {
            canvas.enabled = true;
        }

        // Decprecated; Rely on automatically finding Selectables in menu;
        // foreach (var button in buttons) {
        //     button.enabled = true;
        // }

        // foreach (var selectable in selectables) {
        //     selectable.interactable = true;
        // }

        if (canvasGroup != null) {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        OnShowMenu.Invoke();
        // override stuff here to be turned on when canvas is enabled.
    }

    // Refactoring; Alias
    public virtual void ShowMenu() {
        ShowCanvas();
    }

    public virtual void HideCanvas() {
        if (canvas != null) {
            canvas.enabled = false;
        }

        // Decprecated; Rely on automatically finding Selectables in menu;
        // foreach (var button in buttons) {
        //     button.enabled = false;
        // }

        // foreach (var selectable in selectables) {
        //     selectable.interactable = false;
        // }

        if (canvasGroup != null) {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        OnHideMenu.Invoke();
        // override stuff here to be shut off when canvas is disabled.
    }

    // public virtual void SelectButton() {
    //     if (defaultSelectedButton != null) {
    //         defaultSelectedButton.Select();
    //         return;
    //     }

    //     if (buttons.Count > 0) {
    //         buttons[0].Select();
    //         return;
    //     }
        
    //     Button firstFoundButton = GetComponentInChildren<Button>();

    //     if (firstFoundButton == null) {
    //         return;
    //     }

    //     firstFoundButton.Select();
    // }

    public virtual void HideMenu() {
        HideCanvas();
    }

    public virtual void SelectDefaultElement() {
        if (defaultSelection != null) {
            defaultSelection.Select();
            return;
        }

        Selectable foundSelectable = GetComponentInChildren<Selectable>();

        if (foundSelectable == null) {
            return;
        }

        foundSelectable.Select();
    }
}
