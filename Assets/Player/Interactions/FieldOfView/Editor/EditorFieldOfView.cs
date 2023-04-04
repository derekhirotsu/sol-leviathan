using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This allows the FieldOfView to be drawn as a gizmo in Scene View
[CustomEditor ( typeof(FieldOfView) )]
public class EditorFieldOfView : Editor {
    void OnSceneGUI() {
        FieldOfView fov = (FieldOfView) target;

        // Draw the circle in a radius around the object
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);

        if (fov.ViewAngle > 0) {
            if (!fov.ViewExpandsClockwise) {
                Vector3 viewAngleA = fov.DirFromAngle(-fov.ViewAngle/2, false);
                Vector3 viewAngleB = fov.DirFromAngle(fov.ViewAngle/2, false);

                Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
                Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);
            } else {
                Vector3 viewAngleA = fov.DirFromAngle(0, false);
                Vector3 viewAngleB = fov.DirFromAngle(fov.ViewAngle, false);

                Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
                Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);
            }
            
        }
        
    }
}
