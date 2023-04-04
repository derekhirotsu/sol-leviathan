using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour {
    TMP_Text TextElement;

    void Start() {
        TextElement = GetComponent<TMP_Text>();
        TextElement.text = "Ver. " + Application.version;
    }
}
