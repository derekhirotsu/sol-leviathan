using UnityEngine;

public abstract class MainCameraTracker : MonoBehaviour {
    [SerializeField]
    protected MainCameraRef mainCameraRef;

    protected virtual void OnEnable() {
        mainCameraRef.MainCameraChange += OnMainCameraChange;
    }

    protected virtual void OnDisable() {
        mainCameraRef.MainCameraChange -= OnMainCameraChange;
    }

    protected abstract void OnMainCameraChange();
}
