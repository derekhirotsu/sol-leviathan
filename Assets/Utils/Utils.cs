using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // --
    // Vector3
    // --
    public static float SqrDistance(Vector3 a, Vector3 b) {
        return (b - a).sqrMagnitude;
    }

    public static bool WithinRangeSqr(Vector3 a, Vector3 b, float range) {
        return SqrDistance(a, b) < Mathf.Pow(range, 2);
    }


}
