using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuDropdownController : MonoBehaviour {
    [SerializeField]
    Menu2 menu;

    TMP_Dropdown[] dropdowns;

    void Start() {
        dropdowns = GetComponentsInChildren<TMP_Dropdown>();
    }

    void OnEnable() {
        menu.OnHide.AddListener(HideDropdowns);
    }

    void OnDisable() {
        menu.OnHide.RemoveListener(HideDropdowns);
    }

    // This is to prevent menu closing without closing dropdowns first.
    public void HideDropdowns() {
        foreach (var dropdown in dropdowns) {
            if (dropdown.IsExpanded) {
                // Set dropdown to hide instantly.
                var tempAlphaFade = dropdown.alphaFadeSpeed;
                dropdown.alphaFadeSpeed = 0;
                dropdown.Hide();

                // Restore original fade speed.
                dropdown.alphaFadeSpeed = tempAlphaFade;
            }
        }
    }
}
