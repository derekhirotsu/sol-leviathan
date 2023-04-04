using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MainCameraRef")]
public class MainCameraRef : ScriptableObject {
    protected Camera mainCamera;
    public Camera MainCamera { get { return mainCamera; } }

    public event Action MainCameraChange;

    public void SetMainCamera(Camera newCamera) {
        mainCamera = newCamera;
        MainCameraChange?.Invoke();
    }
}
