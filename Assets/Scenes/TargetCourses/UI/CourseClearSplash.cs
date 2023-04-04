using System.Collections;
using UnityEngine;

public class CourseClearSplash : MonoBehaviour {
    [SerializeField]
    RectTransform resultRectTransform;

    RectTransform rectTransform;
    TypingText typingText;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        typingText = GetComponentInChildren<TypingText>();
    }

    public IEnumerator DisplayUI() {
        yield return StartCoroutine(typingText.TypeText());
        yield return new WaitForSeconds(0.3f);

        float elapsedTime = 0f;
        float completionTime = 1.3f;
        while(Mathf.Abs(rectTransform.sizeDelta.sqrMagnitude) > Mathf.Abs(resultRectTransform.sizeDelta.sqrMagnitude)) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / completionTime;
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, resultRectTransform.sizeDelta, t);
            // if (rectTransform.sizeDelta.y + 10 > resultRectTransform.sizeDelta.y) {
            //     rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, resultRectTransform.sizeDelta.y);
            // } else {
            //     rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + (10f * Time.deltaTime));
            // }
            // rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + (100f * Time.deltaTime));
            // rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, resultRectTransform.sizeDelta, Time.deltaTime);
            yield return null;
        }

        rectTransform.sizeDelta = resultRectTransform.sizeDelta;
    }
}
