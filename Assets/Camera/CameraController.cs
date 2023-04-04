using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    
    [Header("Tunable Paramaters")]
    [SerializeField] protected Camera cam;
    [SerializeField] protected MainCameraRef cameraRef;
    [Range(0.01f, 1f)] [SerializeField] protected float camFollowSpeed = 1f;
    [SerializeField] protected Image flashOverlay;
    [SerializeField] protected Transform playerPosition;
    [SerializeField] protected Vector3 cameraLocationOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (playerPosition is null) {
            this.gameObject.SetActive(false);
        }

        if (cameraRef != null) {
            cameraRef.SetMainCamera(cam);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowPlayerPosition();
    }

    protected void FollowPlayerPosition() {
        Vector3 targetTransform = Vector3.Lerp(this.transform.position + cameraLocationOffset, playerPosition.position + cameraLocationOffset, camFollowSpeed);

        this.transform.position = targetTransform;
    }
    

    // ==== HITSTOP
    protected IEnumerator hitstopCoroutine;
    public void Hitstop(float duration) {
        if (cam != null) {
            StartCoroutine(HitStopForDuration(duration));
        }
    }

    public IEnumerator HitStopForDuration(float duration) {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        hitstopCoroutine = null;
        Time.timeScale = 1f;
    }
    
    // ==== CAMERA SHAKE
    public void CameraShake(float duration, float magnitude) {
        if (cam != null) {
            StartCoroutine(ShakeForDuration(duration, magnitude));
        }
    }

    protected IEnumerator ShakeForDuration(float duration, float magnitude) {
        Quaternion og = cam.transform.localRotation;

        while (duration >= 0.0f) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, z);

            duration -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        cam.transform.localPosition = Vector3.zero;
    }

    // ==== CAMERA FLASH
    public void CameraFlash(float speed) {
        if (flashOverlay != null) {
            StartCoroutine(FlashForDuration(speed));
        }
    }

    protected IEnumerator FlashForDuration(float flashSpeed) {
        while (flashOverlay.color.a < 1) {
            float newAlpha = flashOverlay.color.a + (flashSpeed * Time.unscaledDeltaTime);
            if (newAlpha > 1) {
                newAlpha = 1;
            }

            Color newColor =  new Color(flashOverlay.color.r, flashOverlay.color.g, flashOverlay.color.b, newAlpha);
            flashOverlay.color = newColor;
            yield return new WaitForEndOfFrame();
        }

        while (flashOverlay.color.a > 0) {
            float newAlpha = flashOverlay.color.a - ((flashSpeed * 1.5f) * Time.unscaledDeltaTime);
            if (newAlpha < 0) {
                newAlpha = 0;
            }
            Color newColor =  new Color(flashOverlay.color.r, flashOverlay.color.g, flashOverlay.color.b, newAlpha);
            flashOverlay.color = newColor;
            yield return new WaitForEndOfFrame();
        }
        // wh
    }
}
