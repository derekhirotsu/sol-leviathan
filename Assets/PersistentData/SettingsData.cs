using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableVariables;

[CreateAssetMenu(menuName = "SettingsData")]
public class SettingsData : ScriptableObject {
    public IntVariable GraphicsLevel;
    public FloatVariable MasterVolume;
    public FloatVariable MusicVolume;
    public FloatVariable SfxVolume;

    void OnEnable() {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public SettingsSaveData ToSettingsSaveData() {
        return new SettingsSaveData(
            GraphicsLevel.Value,
            MasterVolume.Value,
            MusicVolume.Value,
            SfxVolume.Value
        );
    }

    public void ApplySaveData(SettingsSaveData saveData) {
        GraphicsLevel.SetValue(saveData.GraphicsLevel);
        MasterVolume.SetValue(saveData.MasterVolume);
        MusicVolume.SetValue(saveData.MusicVolume);
        SfxVolume.SetValue(saveData.SfxVolume);
    }
}
