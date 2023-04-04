using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_HealthTracker : MonoBehaviour
{
    [SerializeField] protected HitDetection.HealthController healthModule;

    [Header("UI Fields")]
    [SerializeField] protected Image fillBar;
    [SerializeField] protected TMP_Text fillText;

    void Start() {
        if (healthModule == null) {
            Debug.LogWarning(this.name + " was not given a HealthController.");
            gameObject.SetActive(false);
            // this.enabled = false;
        }
        
        fillBar.fillAmount = healthModule.CurrentHealth / healthModule.MaxHealth;
        fillText.text = healthModule.CurrentHealth.ToString();
    }

    // void Update() {
    //     fillBar.fillAmount = healthModule.CurrentHealthPercentage;
    //     fillText.text = healthModule.CurrentHealth.ToString();
    // }

    void OnEnable() {
        healthModule.CurrentHealthChange += OnCurrentHealthChange;
    }

    void OnDisable() {
        healthModule.CurrentHealthChange -= OnCurrentHealthChange;
    }

    void OnCurrentHealthChange() {
        fillBar.fillAmount = healthModule.CurrentHealth / healthModule.MaxHealth;
        fillText.text = healthModule.CurrentHealth.ToString();
    }
}
