using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanel : MonoBehaviour {
    [SerializeField]
    GameObject upgradeList;

    void Update() {
        if (Keyboard.current.backquoteKey.wasReleasedThisFrame) {
            upgradeList.SetActive(!upgradeList.activeInHierarchy);
        }
    }

}
