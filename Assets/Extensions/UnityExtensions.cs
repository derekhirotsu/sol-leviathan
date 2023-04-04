using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{
    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }


    // ---
    //  Vector2
    // ---
    public static Vector2 IsolateX(this Vector2 vector) {
        return new Vector2(vector.x, 0);
    }

    public static Vector2 IsolateY(this Vector2 vector) {
        return new Vector2(0, vector.y);
    }
    
}
