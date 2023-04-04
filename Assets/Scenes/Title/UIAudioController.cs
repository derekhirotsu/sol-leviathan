using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableVariables;

[System.Serializable]
public class UIAudioClip {
    public AudioClip clip;
    public string clipName;
    public float baseVolume;
    public float minPitch;
    public float maxPitch;
}

public class UIAudioController : MonoBehaviour {
    [SerializeField]
    ScriptableVariableReference<float> sfxVolume;

    [SerializeField]
    List<UIAudioClip> audioClips;

    Dictionary<string, UIAudioClip> clipLookup;

    AudioSource audioSource;

    public static bool ShouldPlaySelectAudio;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        clipLookup = new Dictionary<string, UIAudioClip>();

        foreach (var clip in audioClips) {
            if (clipLookup.ContainsKey(clip.clipName)) {
                continue;
            }

            clipLookup.Add(clip.clipName, clip);
        }
    }

    public void PlayClip(int clipIndex) {
        if (clipIndex < 0 || clipIndex >= audioClips.Count) {
            Debug.LogWarning($"Index: {clipIndex} not valid.", this);
            return;
        }

        UIAudioClip clip = audioClips[clipIndex];

        if (clip == null) {
            return;
        }

        audioSource.pitch = Random.Range(clip.minPitch, clip.maxPitch);
        audioSource.PlayOneShot(clip.clip, (clip.baseVolume * sfxVolume) / 100);
        audioSource.pitch = 1f;
    }

    public void PlayClip(string clipName) {
        UIAudioClip clip;
        if (!clipLookup.TryGetValue(clipName, out clip)) {
            Debug.LogWarning($"Audio clip: {clipName} not found.", this);
            return;
        }

        if (clip == null) {
            return;
        }

        audioSource.pitch = Random.Range(clip.minPitch, clip.maxPitch);
        audioSource.PlayOneShot(clip.clip, (clip.baseVolume * sfxVolume) / 100);
        audioSource.pitch = 1f;
    }

    public void PlaySelectAudio() {
        if (!ShouldPlaySelectAudio) {
            ShouldPlaySelectAudio = true;
            return;
        }

        PlayClip("select");
    }
}
