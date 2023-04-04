using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour {
    [Header("Input Source Reference")]

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> gamepadInputActive;

    [SerializeField]
    protected List<Menu> menus;

    [SerializeField]
    protected Menu primaryMenu;

    protected Menu currentMenu;

    protected Dictionary<string, Menu> menuLookup;

    void OnEnable() {
        gamepadInputActive.Subscribe(OnGamepadInputActiveChange);
    }

    void OnDisable() {
        gamepadInputActive.Unsubscribe(OnGamepadInputActiveChange);
    }

    void Awake() {
        menuLookup = new Dictionary<string, Menu>();

        foreach (var menu in menus) {
            if (menuLookup.ContainsKey(menu.Slug)) {
                menu.HideCanvas();
            } else {
                menuLookup.Add(menu.Slug, menu);
            }
        }
    }

    public void ShowMenu(string slug, bool hidePrimaryMenu = false) {
        Menu nextMenu;
        if (!menuLookup.TryGetValue(slug, out nextMenu)) {
            return;
        }

        if (nextMenu == null) {
            return;
        }

        foreach (var menu in menus) {
            menu.HideCanvas();
        }

        if (!hidePrimaryMenu) {
            primaryMenu.ShowCanvas();
        }

        currentMenu = nextMenu;
        currentMenu.ShowCanvas();

        if (gamepadInputActive.Value) {
            currentMenu.SelectDefaultElement();
        }
    }

    public void ShowSingleMenu(string slug) {
        ShowMenu(slug, true);
    }

    public void ShowSingleMenu(int index) {
        ShowMenu(index, true);
    }

    public void ShowMenu(int index, bool hidePrimaryMenu = false) {
        if (index < 0 || index >= menus.Count) {
            return;
        }

        Menu nextMenu = menus[index];
        
        if (nextMenu == null) {
            return;
        }

        foreach (var menu in menus) {
            menu.HideCanvas();
        }

        if (!hidePrimaryMenu) {
            primaryMenu.ShowCanvas();
        }

        currentMenu = nextMenu;
        currentMenu.ShowCanvas();

        if (gamepadInputActive.Value) {
            currentMenu.SelectDefaultElement();
        }  
    }

    public void HideMenus() {
        foreach (var menu in menus) {
            menu.HideCanvas();
        }
    }

    protected void OnGamepadInputActiveChange(bool newValue) {
        if (!newValue) {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        if (currentMenu != null) {
            // currentMenu.SelectButton();
            currentMenu.SelectDefaultElement();
        }
    }

    // These static functions probably can be moved somewhere else later...

    public static void ReloadCurrentScene() {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public static void LoadSceneByName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    
    public static void QuitGame() {
        Application.Quit();
    }
}
