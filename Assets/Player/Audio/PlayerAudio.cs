using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    [SerializeField]
    protected List<PlayerAudioClips> playerAudioClips;

    protected Dictionary<string, PlayerAudioClips> audioLookup;

    protected AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioLookup = new Dictionary<string, PlayerAudioClips>();

        foreach (var clip in playerAudioClips) {
            if (!audioLookup.ContainsKey(clip.ClipName)) {
                audioLookup.Add(clip.ClipName, clip);
            }
        }
    }

    public void PlayClipOneShot(string clipName) {
        if (!audioLookup.ContainsKey(clipName)) {
            return;
        }

        var clip = audioLookup[clipName];
        
        audioSource.pitch = Random.Range(clip.MinPitchValue, clip.MaxPitchValue);
        audioSource.PlayOneShot(clip.audioClip, clip.Volume);
    }
}

[System.Serializable]
public class PlayerAudioClips {
    public string ClipName;
    public AudioClip audioClip;
    public float MinPitchValue = 1f;
    public float MaxPitchValue = 1f;
    public float Volume = 1f;
}
