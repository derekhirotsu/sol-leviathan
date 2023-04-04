using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingText : MonoBehaviour {
    [SerializeField]
    TMP_Text TextElement;

    [SerializeField]
    string TextCursor = "<mark=#FFFFFF>_</mark>";

    [SerializeField]
    float DelayBetweenCharacters;

    [SerializeField]
    float BlinkRate;

    [SerializeField]
    int Blinks;

    // [SerializeField]
    // AudioClip TypingSfx;

    // AudioSource audioSource;

    // State
    string FullText;
    WaitForSeconds WaitForTypingDelay;
    WaitForSeconds WaitForBlinkDelay;

    void Start() {
        // audioSource = GetComponent<AudioSource>();
        FullText = TextElement.text;
        WaitForTypingDelay = new WaitForSeconds(DelayBetweenCharacters);
        WaitForBlinkDelay = new WaitForSeconds(BlinkRate);
    }

    public void SetFullText(string text) {
        FullText = text;
    }

    public void ResetText() {
        TextElement.text = FullText;
    }

    public void ClearText() {
        TextElement.text = "";
    }

    public IEnumerator TypeText() {
        for (int i = 0; i < FullText.Length; i++) {
            TextElement.text = FullText.Substring(0, i) + TextCursor;
            // audioSource.pitch = Random.Range(0.95f, 1.05f);
            // audioSource.PlayOneShot(TypingSfx, 0.2f);
            yield return WaitForTypingDelay;
        }

        TextElement.text = FullText;

        for (int i = 0; i < Blinks; i++) {
            TextElement.text = FullText + TextCursor;
            yield return WaitForBlinkDelay;
            TextElement.text = FullText;
            yield return WaitForBlinkDelay;
        }
    }
}
