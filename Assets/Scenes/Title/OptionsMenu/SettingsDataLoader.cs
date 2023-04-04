using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDataLoader : MonoBehaviour {
    [SerializeField]
    SettingsData settings;

    static bool SettingsInitialized;

    void Start() {
        InitializeSettingsData();
    }

    void InitializeSettingsData() {
        if (SettingsInitialized) {
            return;
        }

        if (!PersistentData.TryValidateRootDirectory()) {
            return;
        }

        SettingsSaveData defaultSettings = new SettingsSaveData();
        SettingsSaveData loadedSettings = PersistentData.TryLoadSettingsData();

        // an issue occurred with loading settings data.
        if (loadedSettings == null) {
            // settings.ApplySaveData(defaultSettings);
        } else {
            // settings.ApplySaveData(loadedSettings);
        }

        SettingsInitialized = true;
    }
}
