using UnityEngine;
using TMPro;

public class TargetCounter : MonoBehaviour {
    [SerializeField]
    protected EntityLookup activeTargets;

    [SerializeField]
    protected TMP_Text counterLabel;

    [SerializeField]
    protected TMP_Text counterText;

    void Start() {
        if (activeTargets == null) {
            counterLabel.text = $"No Targets Found";
            counterText.enabled = false;
            enabled = false;
        } else {
            counterLabel.text = "Targets:";
        }
    }

    void Update() {
        if (activeTargets.ItemCount == 0) {
            counterLabel.text = $"No Targets";
            counterText.enabled = false;   
            return;
        }

        counterText.text = activeTargets.ItemCount.ToString();        
    }
}
