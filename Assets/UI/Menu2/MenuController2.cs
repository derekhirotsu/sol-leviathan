using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ScriptableVariables;

public class MenuController2 : MonoBehaviour {
    [SerializeField]
    InputActionAsset actionAsset;

    [SerializeField]
    List<Menu2> menus;

    Menu2 activeMenu;

    Dictionary<string, Menu2> menuLookup;

    InputActionMap actionMap;

    void Awake() {
        menuLookup = new Dictionary<string, Menu2>();

        foreach (var menu in menus) {
            if (menu == null) {
                continue;
            }
            if (menuLookup.ContainsKey(menu.Slug)) {
                Debug.LogWarning("Duplicate menu key encountered.", menu);
                menu.Hide();
                continue;
            }

            menuLookup.Add(menu.Slug, menu);
        }

        actionMap = actionAsset.FindActionMap("UI");
    }

    void OnEnable() {
        if (actionMap == null) {
            return;
        }

        actionMap["Submit"].performed += OnSubmit;
        actionMap["Cancel"].performed += OnCancel;
        actionMap["TabLeft"].performed += OnTabLeft;
        actionMap["TabRight"].performed += OnTabRight;
        actionMap["Navigate"].performed += OnNavigate;   
        actionMap["Click"].performed += OnClick;   
    }

    void OnDisable() {
        if (actionMap == null) {
            return;
        }

        actionMap["Submit"].performed -= OnSubmit;
        actionMap["Cancel"].performed -= OnCancel;
        actionMap["TabLeft"].performed -= OnTabLeft;
        actionMap["TabRight"].performed -= OnTabRight;
        actionMap["Navigate"].performed -= OnNavigate;
        actionMap["Click"].performed -= OnClick;
    }

#region Menu Controller Public API

    /// <summary>
    /// Sets and shows the active menu.
    /// All other menus controlled by this component will be hidden if they
    /// aren't already.
    /// </summary>
    void ShowMenu(Menu2 nextMenu) {
        if (nextMenu == null) {
            return;
        }

        activeMenu = nextMenu;

        foreach (var menu in menus) {
            if (menu == activeMenu || menu == null) {
                continue;
            }

            menu.Hide();
        }
        
        activeMenu.Show();
        activeMenu.SelectElement();
    } 

    /// <summary>
    /// Given an index, gets a menu from the list of menus and shows it.
    /// All other menus controlled by this component will be hidden if they
    /// aren't already.
    /// </summary>
    public void ShowMenu(int index) {
        if (index < 0 || index >= menus.Count) {
            return;
        }

        ShowMenu(menus[index]);
    }

    /// <summary>
    /// Given a string slug, gets a menu with the corresponding slug and shows it.
    /// All other menus controlled by this component will be hidden if they
    /// aren't already.
    /// </summary>
    public void ShowMenu(string slug) {
        Menu2 nextMenu;
        if (!menuLookup.TryGetValue(slug, out nextMenu)) {
            return;
        } 

        ShowMenu(nextMenu);
    }

    /// <summary>
    /// Shhows a menu and sets it as the active menu. Will not hide any other showing menus.
    /// </summary>
    void ShowMenuAdditive(Menu2 nextMenu) {
        if (nextMenu == null) {
            return;
        }

        // If next menu is already showing, return so active menu is not set to this menu.
        if (!nextMenu.IsHidden) {
            return;
        }

        activeMenu = nextMenu;
        activeMenu.Show();
        activeMenu.SelectElement();
    }

    /// <summary>
    /// Given an index, gets a menu from the list of menus, shows it and sets
    /// it as the active menu. Will not hide any other showing menus.
    /// </summary>
    public void ShowMenuAdditive(int index) {
        if (index < 0 || index >= menus.Count) {
            return;
        }

        ShowMenuAdditive(menus[index]);
    }

    /// <summary>
    /// Given a string slug, gets a menu with the corresponding slug, shows it and sets
    /// it as the active menu. Will not hide any other showing menus.
    /// </summary>
    public void ShowMenuAdditive(string slug) {
        Menu2 nextMenu;
        if (!menuLookup.TryGetValue(slug, out nextMenu)) {
            return;
        } 

        ShowMenuAdditive(nextMenu);
    }

    /// <summary>
    /// Shows a set of menus. Hides all other menus. The last menu in the list
    /// of menus to select will be set as the active menu.
    /// </summary>
    void ShowMenus(List<Menu2> selectedMenus) {
        if (!(selectedMenus.Count > 0)) {
            return;
        }

        foreach (var menu in menus) {
            if (menu == null) {
                continue;
            }
            if (!selectedMenus.Contains(menu)) {
                menu.Hide();
            } else {
                menu.Show();

                // Ensure activeMenu is set to last valid menu in selectedMenus list.
                activeMenu = menu;
            }
        }

        activeMenu.SelectElement();
    }

    /// <summary>
    /// Shows a set of menus based on the provided indexes.
    /// All other menus controlled by this component will be hidden if they
    /// aren't already.
    /// Indexes not in the list of menus are ignored.
    /// </summary>
    public void ShowMenus(params int[] indexes) {
        List<Menu2> selectedMenus = new List<Menu2>();

        foreach (int index in indexes) {
            if (index < 0 || index >= menus.Count) {
                continue;
            }

            if (menus[index] == null) {
                continue;
            }

            selectedMenus.Add(menus[index]);
        }

        ShowMenus(selectedMenus);      
    }

    /// <summary>
    /// Shows a set of menus based on the provided slugs.
    /// All other menus controlled by this component will be hidden if they
    /// aren't already.
    /// Slugs that do not have a corresponding menu are ignored.
    /// </summary>
    public void ShowMenus(params string[] slugs) {
        List<Menu2> selectedMenus = new List<Menu2>();

        foreach (string slug in slugs) {
            Menu2 nextMenu;
            if (!menuLookup.TryGetValue(slug, out nextMenu)) {
                continue;
            }

            if (nextMenu == null) {
                continue;
            }

            selectedMenus.Add(nextMenu);
        }

        ShowMenus(selectedMenus);
    }

    /// <summary>
    /// Hides a menu based on the provided slug.
    /// If the hidden menu is the active menu, the active menu will be set to null.
    /// Slugs that do not have a corresponding menu are ignored.
    /// </summary>
    public void HideMenu(string slug) {
        Menu2 nextMenu;
        if (!menuLookup.TryGetValue(slug, out nextMenu)) {
            return;
        } 

        if (nextMenu == null) {
            return;
        }

        if (nextMenu.IsHidden) {
            return;
        }

        if (activeMenu == nextMenu) {
            activeMenu = null;
        }

        nextMenu.Hide();
    }

    /// <summary>
    /// Hides a menu based on the provided index.
    /// If the hidden menu is the active menu, the active menu will be set to null.
    /// Indexes not in the list of menus are ignored.
    /// </summary>
    public void HideMenu(int index) {
        if (index < 0 || index >= menus.Count) {
            return;
        }

        Menu2 nextMenu = menus[index];

        if (nextMenu == null) {
            return;
        }

        if (nextMenu.IsHidden) {
            return;
        }

        if (activeMenu == nextMenu) {
            activeMenu = null;
        }

        nextMenu.Hide();
    }

    /// <summary>
    /// Convenience method to hide all menus controlled by this component.
    /// Menus that are already hidden will not have their Hide method called.
    /// Sets the active menu to null.
    /// </summary>
    public void HideAllMenus() {
        foreach (var menu in menus) {
            if (menu == null) {
                continue;
            }

            menu.Hide();
        }

        activeMenu = null;
    }

    /// <summary>
    /// Convenience method to show all menus controlled by this component.
    /// Menus that are already showing will not have their Show method called.
    /// Sets the active menu to the last valid menu in the list.
    /// </summary>
    public void ShowAllMenus() {
        foreach (var menu in menus) {
            if (menu == null) {
                continue;
            }

            menu.Show();
            activeMenu = menu;
        }
    }

#endregion

#region InputSystem Methods

    public void EnableActions() {
        if (actionMap == null || actionMap.enabled) {
            return;
        }

        actionMap.Enable();
    }

    public void DisableActions() {
        if (actionMap == null || !actionMap.enabled) {
            return;
        }

        actionMap.Disable();        
    }

    void OnSubmit(InputAction.CallbackContext context) {
        if (activeMenu == null) {
            return;
        }

        activeMenu.Submit(context);
    }

    void OnCancel(InputAction.CallbackContext context) {
        if (activeMenu == null) {
            return;
        }
        
        activeMenu.Cancel(context);
    }

    void OnTabLeft(InputAction.CallbackContext context) {
        if (activeMenu == null) {
            return;
        }
        
        activeMenu.TabLeft(context);
    }

    void OnTabRight(InputAction.CallbackContext context) {
        if (activeMenu == null) {
            return;
        }
        
        activeMenu.TabRight(context);
    }

    void OnClick(InputAction.CallbackContext context) {
        if (activeMenu == null) {
            return;
        }
        
        activeMenu.Click(context);   
    }

    void OnNavigate(InputAction.CallbackContext context) {
        if (activeMenu == null) {
            return;
        }
        
        activeMenu.Navigate(context);
    }

#endregion

}


