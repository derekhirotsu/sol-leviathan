using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionSlider : MonoBehaviour {
    [SerializeField]
    TMP_Text sliderLabel;

    [SerializeField]
    Slider slider;

    void Start() {
        // This will read in from persistent option data at some point
        sliderLabel.text = slider.value.ToString();
    }

    public void OnValueChanged(Slider slider) {
        sliderLabel.text = slider.value.ToString();
    }
}
