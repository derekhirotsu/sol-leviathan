using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudio : MonoBehaviour {
    [SerializeField]
    protected float pitchVariance = 0f;

    protected AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();  
    }

    public void PlayDetached() {
        gameObject.transform.parent = null;

        audioSource.pitch = Random.Range(1 - pitchVariance, 1 + pitchVariance);
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
    }
}
