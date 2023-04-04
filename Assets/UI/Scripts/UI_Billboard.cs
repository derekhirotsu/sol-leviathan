using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Billboard : MonoBehaviour
{
    protected Transform targetCamera;

    void Start() {
        Camera mainCam = Camera.main;
        if (mainCam != null) {
            targetCamera = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (targetCamera != null) {
            transform.LookAt(transform.position + targetCamera.forward);
        }
    }
}
