using UnityEngine;

// Used with a World Space Canvas to cache/track a reference
// to the current main camera in the scene.
public class CanvasCameraTracker : MainCameraTracker {
    [SerializeField]
    protected Canvas canvas;

    void Start() {
        canvas.worldCamera = mainCameraRef.MainCamera;
    }

    protected override void OnMainCameraChange() {
        canvas.worldCamera = mainCameraRef.MainCamera;
    }
}
