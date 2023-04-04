using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableVariables;
using TMPro;

public class BackHint : MonoBehaviour {
    [SerializeField]
    protected TMP_Text hintDisplay;

    [SerializeField]
    protected string gamepadSprite;

    [SerializeField]
    protected string keyboardSprite;

    [SerializeField]
    protected ScriptableVariableReference<bool> gamepadActive;

    string currentSprite;

    string hintText {
        get { return "Back - " +  currentSprite; }
    }

    void Start() {
        OnControlChange(gamepadActive.Value);
    }

    void OnEnable() {
        gamepadActive.Subscribe(OnControlChange);
    }

    void OnDisable() {
        gamepadActive.Unsubscribe(OnControlChange);
    }

    void OnControlChange(bool newValue) {
        currentSprite = newValue ? gamepadSprite : keyboardSprite;

        hintDisplay.text = hintText;
    }
}
