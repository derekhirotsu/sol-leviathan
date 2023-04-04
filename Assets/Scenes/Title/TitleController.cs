using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ScriptableVariables;

public class TitleController : MonoBehaviour {
    [SerializeField]
    BoolVariable gamepadInputActive;

    MenuController2 menuController;

    void Start() {
        menuController = GetComponent<MenuController2>();

        // menuController.ShowMenu("main");

        // ----- THIS IS FOR THE ALPHA -----
        if (AlphaMenuController.messageShown) {
            menuController.ShowMenu("main");
            return;
        }

        menuController.ShowMenu("alpha_intro");
        // ---------------------------------
    }

    public void QuitGame() {
        SceneController.QuitGame();
    }

    public void OnControlsChanged(PlayerInput input) {
        bool isGamepadCurrentScheme = input.currentControlScheme == "Gamepad";

        if (isGamepadCurrentScheme != gamepadInputActive.Value) {
            gamepadInputActive.SetValue(isGamepadCurrentScheme);
        }
    }
}
