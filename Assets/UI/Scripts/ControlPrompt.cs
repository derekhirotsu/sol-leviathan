using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ControlPrompt : MonoBehaviour {
    [SerializeField]
    int gamepadSpriteIndex;

    [SerializeField]
    int keyboardSpriteIndex;

    [SerializeField]
    [Multiline]
    string controlText;

    TMP_Text textElement;
    bool showGamepadSprite;

    void Start() {
        textElement = GetComponent<TMP_Text>();

        if (textElement == null) {
            gameObject.SetActive(false);
            return;
        }

        UpdateText();
    }

    public void OnControlsChanged(PlayerInput playerInput) {
        showGamepadSprite = playerInput.currentControlScheme == "Gamepad";
        
        if (textElement == null) {
            return;
        }
        
        UpdateText();
    }

    void UpdateText() {
        int currentSprite = showGamepadSprite ? gamepadSpriteIndex : keyboardSpriteIndex;
        string spriteTag = $"<sprite={currentSprite}>";

        textElement.text = string.Format(controlText, spriteTag);
    }

    public void SetText(string text) {
        controlText = text;
        UpdateText();
    }
}
