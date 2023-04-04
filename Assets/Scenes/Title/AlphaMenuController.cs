using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AlphaMenuController : MonoBehaviour {
    // [SerializeField]
    // Image countdownRight;

    // [SerializeField]
    // Image countdownLeft;

    [SerializeField]
    MenuController2 menuController;

    public static bool messageShown;
    // public bool canSkip;
    // [SerializeField]
    // float messageTime;

    // public void StartDisplayMessage() {
    //     StartCoroutine(DisplayMessage());
    // }

    // public void StopDisplayMessage() {
    //     StopCoroutine(DisplayMessage());
    // }

    // IEnumerator DisplayMessage() {
    //     float waitTime = messageTime;
    //     while (waitTime > 0) {
    //         waitTime -= Time.deltaTime;

    //         countdownRight.fillAmount = waitTime / messageTime;
    //         countdownLeft.fillAmount = waitTime / messageTime;
    //         yield return null;
    //     }

    //     yield return new WaitForSeconds(1f);

    //     menuController.ShowMenu("main");
    //     messageShown = true;
    // }

    public void SkipMessage() {
        menuController.ShowMenu("main");
        messageShown = true;
    }
}
