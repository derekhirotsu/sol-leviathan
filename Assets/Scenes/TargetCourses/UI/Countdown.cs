using System.Collections;
using UnityEngine;
using TMPro;


public class Countdown : MonoBehaviour {
    [SerializeField]
    [Min(1)]
    int startingNumber;

    [SerializeField]
    [Min(0)]
    float countTimeInterval;

    [SerializeField]
    [Min(0)]
    float finalCountInterval;

    TMP_Text textElement;
    WaitForSeconds WaitForCount;
    WaitForSeconds WaitForLastCount;

    void Awake() {
        textElement = GetComponent<TMP_Text>();
        WaitForCount = new WaitForSeconds(countTimeInterval);
        WaitForLastCount = new WaitForSeconds(finalCountInterval);

        textElement.text = startingNumber.ToString();
    }

    public IEnumerator Count() {
        while (startingNumber > 0) {
            textElement.text = startingNumber.ToString();
            startingNumber--;

            yield return WaitForCount;
        }

        textElement.text = "GO!";

        yield return WaitForLastCount;
    }
}
