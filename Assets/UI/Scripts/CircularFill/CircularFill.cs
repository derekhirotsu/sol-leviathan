using UnityEngine;
using UnityEngine.UI;

public class CircularFill : MonoBehaviour {
    [SerializeField]
    protected Image image;

    [SerializeField]
    [Range(0, 1)]
    protected float fillAmount;

    void Start() {
        image.fillAmount = fillAmount;
    }

    public void UpdateFillAmount(float value) {
        fillAmount = value;
        image.fillAmount = fillAmount;
    }
}
