using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ResultsController : MonoBehaviour {

    [SerializeField]
    Sprite courseCompleteSprite;

    [SerializeField]
    Sprite parTimeBeatSprite;

    [SerializeField]
    TypingText timeDisplayHeading;

    [SerializeField]
    TypingText timeDisplayTime;

    [SerializeField]
    TypingText bestTimeDisplayHeading;

    [SerializeField]
    TypingText bestTimeDisplayTime;

    [SerializeField]
    Image newRecordBorder;

    [SerializeField]
    TypingText rankHeading;

    [SerializeField]
    Image rankBorder;

    [SerializeField]
    Image rankIcon;

    [SerializeField]
    GameObject menuControls;

    [SerializeField]
    TMP_Text skipPrompt;

    WaitForSeconds WaitPostHeading;
    float postHeadingDelay = 0.1f;

    WaitForSeconds WaitAfterDisplay;
    float postDisplayDelay = 0.8f;

    IEnumerator resultsDisplayCoroutine;

    TargetCourseController courseController;

    string currentTimeText;
    string bestTimeText;
    Sprite rankIconSprite;

    bool resultsShown;

    void Start() {
        WaitPostHeading = new WaitForSeconds(postHeadingDelay);
        WaitAfterDisplay = new WaitForSeconds(postDisplayDelay);
        courseController = GetComponentInParent<TargetCourseController>();
        resultsShown = false;
    }

    public void StartResultsDisplayCoroutine() {
        if (resultsShown) {
            SkipResultsDisplay();
            return;
        }

        if (resultsDisplayCoroutine != null) {
            StopCoroutine(resultsDisplayCoroutine);
        }

        resultsDisplayCoroutine = DisplayResults();
        StartCoroutine(resultsDisplayCoroutine);
    }

    void HideUiElements() {
        timeDisplayHeading.gameObject.SetActive(false);
        timeDisplayTime.gameObject.SetActive(false);
        bestTimeDisplayHeading.gameObject.SetActive(false);
        bestTimeDisplayTime.gameObject.SetActive(false);
        newRecordBorder.enabled = false;
        rankHeading.gameObject.SetActive(false);
        rankBorder.enabled = false;
        rankIcon.enabled = false;
        menuControls.SetActive(false);  
    }

    /// Shows heading for time display, then resulting time of completed run.
    IEnumerator DisplayTime() {
        timeDisplayTime.SetFullText(currentTimeText);
        timeDisplayHeading.gameObject.SetActive(true);
        yield return StartCoroutine(timeDisplayHeading.TypeText());
        yield return WaitPostHeading;
        timeDisplayTime.gameObject.SetActive(true);
        yield return StartCoroutine(timeDisplayTime.TypeText());
    }

    void SkipDisplayTime() {
        timeDisplayTime.SetFullText(currentTimeText);
        timeDisplayHeading.gameObject.SetActive(true);
        timeDisplayTime.gameObject.SetActive(true);
        timeDisplayHeading.ResetText();
        timeDisplayTime.ResetText();      
    }

    /// Shows heading for best course time, then best time on record.
    IEnumerator DisplayBestTime() {
        bestTimeDisplayTime.SetFullText(bestTimeText);
        bestTimeDisplayHeading.gameObject.SetActive(true);
        yield return StartCoroutine(bestTimeDisplayHeading.TypeText());
        yield return WaitPostHeading;
        bestTimeDisplayTime.gameObject.SetActive(true);
        yield return StartCoroutine(bestTimeDisplayTime.TypeText());
    }

    void SkipDisplayBestTime() {
        bestTimeDisplayTime.SetFullText(bestTimeText);
        bestTimeDisplayHeading.gameObject.SetActive(true);
        bestTimeDisplayTime.gameObject.SetActive(true);
        bestTimeDisplayHeading.ResetText();      
        bestTimeDisplayTime.ResetText();
    }

    IEnumerator DisplayFirstClearTime() {
        bestTimeDisplayHeading.SetFullText("First Clear!");
        bestTimeDisplayTime.SetFullText(currentTimeText);
        yield return StartCoroutine(FlashBorder());
        bestTimeDisplayHeading.gameObject.SetActive(true);
        yield return StartCoroutine(bestTimeDisplayHeading.TypeText());
        yield return WaitPostHeading;
        bestTimeDisplayTime.gameObject.SetActive(true);
        yield return StartCoroutine(bestTimeDisplayTime.TypeText());
    }

    void SkipDisplayFirstClearTime() {
        bestTimeDisplayHeading.SetFullText("First Clear!");
        bestTimeDisplayTime.SetFullText(currentTimeText);
        bestTimeDisplayHeading.gameObject.SetActive(true);
        bestTimeDisplayTime.gameObject.SetActive(true);
        bestTimeDisplayHeading.ResetText();      
        bestTimeDisplayTime.ResetText();
        newRecordBorder.enabled = true;   
    }

    /// Displays time when best time is beaten by current run.
    IEnumerator DisplayNewRecordTime() {
        bestTimeDisplayHeading.SetFullText("New Record!");
        bestTimeDisplayTime.SetFullText(currentTimeText);       
        yield return StartCoroutine(FlashBorder());
        yield return StartCoroutine(bestTimeDisplayHeading.TypeText());
        yield return WaitPostHeading;
        yield return StartCoroutine(bestTimeDisplayTime.TypeText());
    }

    void SkipDisplayNewRecordTime() {
        bestTimeDisplayHeading.SetFullText("New Record!");
        bestTimeDisplayTime.SetFullText(currentTimeText);
        bestTimeDisplayTime.gameObject.SetActive(true);
        bestTimeDisplayHeading.ResetText();      
        bestTimeDisplayTime.ResetText();
        newRecordBorder.enabled = true;   
    }

    /// Makes the border around the rank icon toggle on and off.
    IEnumerator FlashBorder() {
        int flashes = 3;

        WaitForSeconds WaitForFlash = new WaitForSeconds(0.1f);

        for (int i = 0; i < flashes; i++) {
            newRecordBorder.enabled = true;
            yield return WaitForFlash;
            newRecordBorder.enabled = false;
            yield return WaitForFlash;
        }

        newRecordBorder.enabled = true;
    }

    IEnumerator FillIcon() {
        while (rankIcon.fillAmount < 1f) {
            rankIcon.fillAmount += Time.deltaTime * 9;
            yield return null;
        }

        rankIcon.fillAmount = 1f;
    }

    IEnumerator DisplayRank() {
        rankHeading.gameObject.SetActive(true);
        yield return StartCoroutine(rankHeading.TypeText());
        yield return WaitPostHeading;
        rankBorder.enabled = true;
        rankIcon.sprite = rankIconSprite;
        rankIcon.fillAmount = 0f;
        rankIcon.enabled = true;
        yield return WaitPostHeading;
        yield return StartCoroutine(FillIcon());
    }

    void SkipDisplayRank() {
        rankHeading.ResetText();
        rankHeading.gameObject.SetActive(true);
        rankBorder.enabled = true;
        rankIcon.sprite = rankIconSprite;
        rankIcon.enabled = true;
        rankIcon.fillAmount = 1f;
    }

    void ShowMenuControls() {
        skipPrompt.enabled = false;
        menuControls.SetActive(true);
    }

    void SetResultValues() {
        currentTimeText = courseController == null
            ? "Current Time Unavailable"
            : TimeFormat.FormatSeconds(courseController.ElapsedCourseTime);

        bestTimeText = courseController == null
            ? "Best Time Unavailable"
            : TimeFormat.FormatSeconds(courseController.CurrentBestTime);

        if (courseController != null) {
            rankIconSprite = courseController.ParTimeBeat ? parTimeBeatSprite : courseCompleteSprite;   
        } else {
            rankIconSprite = courseCompleteSprite;
        }
    }

    IEnumerator DisplayResults() {
        HideUiElements();
        SetResultValues();

        skipPrompt.enabled = true;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(DisplayTime());

        yield return WaitAfterDisplay;

        if (!courseController.CurrentClearStatus) {
            yield return StartCoroutine(DisplayFirstClearTime());
            
            yield return WaitAfterDisplay;
        } else {
            yield return StartCoroutine(DisplayBestTime());

            yield return WaitAfterDisplay;

            if (courseController != null) {
                if (courseController.BestTimeBeat) {
                    yield return StartCoroutine(DisplayNewRecordTime());
                }
            }

            yield return WaitAfterDisplay;
        }

        StartCoroutine(DisplayRank());
        resultsShown = true;
    }

    public void SkipResultsDisplay() {
        if (resultsDisplayCoroutine == null) {
            return;
        }

        StopAllCoroutines();
        SetResultValues();
        SkipDisplayTime();

        if (!courseController.CurrentClearStatus) {
            SkipDisplayFirstClearTime();
        } else {
            SkipDisplayBestTime();

            if (courseController != null) {
                if (courseController.BestTimeBeat) {
                    SkipDisplayNewRecordTime();
                }
            }
        }

        SkipDisplayRank();

        resultsDisplayCoroutine = null;

        ShowMenuControls();
        resultsShown = true;
    }
}
