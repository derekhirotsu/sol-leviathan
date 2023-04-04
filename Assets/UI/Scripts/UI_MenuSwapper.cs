using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class UI_MenuSwapper : MonoBehaviour
{
    [Header("Input Source Reference")]

    [SerializeField]
    protected ScriptableVariables.ScriptableVariableReference<bool> gamepadInputActive;

    [Header("Menu Panels")]
    [SerializeField] protected int currentMenuIndex = 0;
    [SerializeField] protected GameObject[] menuPanels;

    void Awake() {
        // ToggleMenu(1);
    }

    void OnEnable() {
        // InputBuffer.OnControlsChangedEvent += SelectButton;
        gamepadInputActive.Subscribe(OnGamepadInputActiveChange);
    }

    void OnDisable() {
        // InputBuffer.OnControlsChangedEvent -= SelectButton;
        gamepadInputActive.Unsubscribe(OnGamepadInputActiveChange);
    }

    public void ToggleMenu(int menuId = 0) {

        // Case index out of bounds
        if (menuId > menuPanels.Length) {
            Debug.LogWarning("A menu of index " + (menuId - 1) + " does not exist. ");
            return;
        }

        // Disable menu panels
        foreach (GameObject menu in menuPanels) {
            menu.SetActive(false);
        }

        // Case index is 0
        if (menuId == 0) {
            return;
        }

        DisplayMenu(menuId-1);
    }
    
    public void ToggleMenu(int menuId = 0, bool hidePrimaryMenu = false) {

        // Case index out of bounds
        if (menuId > menuPanels.Length) {
            Debug.LogWarning("A menu of index " + (menuId - 1) + " does not exist. ");
            return;
        }

        // Disable menu panels
        foreach (GameObject menu in menuPanels) {
            menu.SetActive(false);
        }

        // Case index is 0
        if (menuId == 0) {
            return;
        }

        if (!hidePrimaryMenu && menuId != 1) {
            DisplayMenu(0);
        }

        DisplayMenu(menuId-1);
        
    }

    protected void DisplayMenu(int index) {
        // Enable selected menu given it's not null
        GameObject selectedMenu = menuPanels[index];
        if (selectedMenu != null) {
            selectedMenu.SetActive(true);
            currentMenuIndex = index;

            // If gamepad input is active, we want to select the first button in children.
            // SelectButton();

            if (gamepadInputActive.Value) {
                SelectButton();
            }
        }
        
    }

    // Search for the first button in the children of the active canvas-group and Select it.
    // protected void SelectButton() {
    //     if (gamepadInputActive.Value) {
    //         GameObject currentMenu = menuPanels[currentMenuIndex];

    //         Button firstButtonFound = currentMenu.GetComponentInChildren<Button>();
    //         if (firstButtonFound != null) {
    //             firstButtonFound.Select();
    //         }
    //     } 
    // }
    protected void SelectButton() {
        GameObject currentMenu = menuPanels[currentMenuIndex];
        Button firstButtonFound = currentMenu.GetComponentInChildren<Button>();
        
        if (firstButtonFound != null) {
            firstButtonFound.Select();
        }
    }

    protected void OnGamepadInputActiveChange(bool newValue) {
        if (!newValue) {
            return;
        }

        SelectButton();
    }

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
