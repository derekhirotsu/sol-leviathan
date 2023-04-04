using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tab : MonoBehaviour {
    [SerializeField]
    Sprite activeTabSprite;

    [SerializeField]
    Sprite normalTabSprite;

    Image tabImage;
    TMP_Text tabText;

    void Start() {
        tabImage = GetComponent<Image>();
        tabText = GetComponentInChildren<TMP_Text>();
    }

    public virtual void SelectTab() {
        tabImage.sprite = activeTabSprite;
        // tabText.fontStyle = FontStyles.Bold;
    }

    public virtual void DeselectTab() {
        tabImage.sprite = normalTabSprite;
        // tabText.fontStyle = FontStyles.Normal;
    }

    public virtual void SetTabText(string text) {
        tabText.text = text;
    }
}
