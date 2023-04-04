using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAudio : MonoBehaviour {
    [SerializeField]
    protected AudioClip targetHitAudio;

    [SerializeField]
    protected AudioClip targetDestroyedAudio;

    protected AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();  
    }

    public void PlayTargetHitAudio() {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.clip = targetHitAudio;
        audioSource.Play(); 
    }

    public void PlayTargetDestoyedAudio() {
        gameObject.transform.parent = null;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.clip = targetDestroyedAudio;
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
    }
}
