using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuTitle : MonoBehaviour {
    [SerializeField]
    TypingText Title;

    [SerializeField]
    TypingText Subtitle;

    [SerializeField]
    float DelayBetweenText;

    public void StartTypingTitle() {
        StartCoroutine(Title.TypeText());
    }

    public void StopTypingTitle() {
        StopCoroutine(Title.TypeText());
        Title.ResetText();
    }

    public void StartTypingSubtitle() {
        StartCoroutine(Subtitle.TypeText());
    }

    public void StopTypingSubtitle() {
        StopCoroutine(Subtitle.TypeText());
        Subtitle.ResetText();
    }

    public void StartTypingFullTitle() {
        StartCoroutine(TypeFullTitle());
    }

    public void StopTypingFullTitle() {
        StopAllCoroutines();
        StopTypingTitle();
        StopTypingSubtitle();
    }

    IEnumerator TypeFullTitle() {
        Subtitle.ClearText();
        yield return StartCoroutine(Title.TypeText());
        yield return new WaitForSeconds(DelayBetweenText);
        yield return StartCoroutine(Subtitle.TypeText());
    }
}
