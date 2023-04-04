[System.Serializable]
public class SettingsSaveData {
    public int GraphicsLevel;
    public float MasterVolume;
    public float MusicVolume;
    public float SfxVolume;

    public SettingsSaveData() {
        GraphicsLevel = 0;
        MasterVolume = 100f;
        MusicVolume = 100f;
        SfxVolume = 100f;
    }

    public SettingsSaveData(int graphics, float master, float music, float sfx) {
        GraphicsLevel = graphics;
        MasterVolume = master;
        MusicVolume = music;
        SfxVolume = sfx;
    }

}
