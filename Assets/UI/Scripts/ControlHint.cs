using UnityEngine;
using TMPro;
using ScriptableVariables;

public class ControlHint : MonoBehaviour {
    [SerializeField]
    ScriptableVariableReference<bool> gamepadInputActive;

    [SerializeField]
    ControlSpriteIndex gamepadControlSprite;

    [SerializeField]
    ControlSpriteIndex keyboardControlSprite;

    [SerializeField]
    [Multiline]
    [Tooltip("This text will be displayed by the text element. Sprite tag will replace {0} in text.")]
    string controlText = "{0}";

    TMP_Text textElement;

    void OnEnable() {
        gamepadInputActive.Subscribe(OnGamepadInputActiveChange);
    }

    void OnDisable() {
        gamepadInputActive.Unsubscribe(OnGamepadInputActiveChange);
    }

    void Start() {
        textElement = GetComponent<TMP_Text>();
        OnGamepadInputActiveChange(gamepadInputActive.Value);
    }

    void OnGamepadInputActiveChange(bool gamepadActive) {
        if (textElement == null) {
            return;
        }

        var currentSpriteIndex = gamepadActive 
            ? gamepadControlSprite
            : keyboardControlSprite;

        string spriteTag = ControlSprites.GetSpriteTag(currentSpriteIndex);
        textElement.text = string.Format(controlText, spriteTag);
    }
}
