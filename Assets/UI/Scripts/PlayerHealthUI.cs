using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour {
    [SerializeField]
    EntityLookup playerEntityLookup;
    HitDetection.HealthController playerHealthModule;

    [Header("UI Fields")]
    [SerializeField] protected Image fillBar;
    [SerializeField] protected TMP_Text fillText;

    void Start() {
        if (playerEntityLookup == null || playerEntityLookup.ItemCount == 0) {
            gameObject.SetActive(false);
            return;
        }

        playerHealthModule = playerEntityLookup.Items[0];
        fillBar.fillAmount = playerHealthModule.CurrentHealth / playerHealthModule.MaxHealth;
        fillText.text = playerHealthModule.CurrentHealth.ToString();
    }


    void OnEnable() {
        if (playerHealthModule == null) {
            return;
        }

        playerHealthModule.CurrentHealthChange += OnCurrentHealthChange;
    }

    void OnDisable() {
        if (playerHealthModule == null) {
            return;
        }

        playerHealthModule.CurrentHealthChange -= OnCurrentHealthChange;
    }

    void OnCurrentHealthChange() {
        fillBar.fillAmount = playerHealthModule.CurrentHealth / playerHealthModule.MaxHealth;
        fillText.text = playerHealthModule.CurrentHealth.ToString();
    }
}
