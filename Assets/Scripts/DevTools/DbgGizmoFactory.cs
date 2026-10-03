using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory for temporary debug gizmos.
/// Gizmos are added through static methods and rendered by this component's OnDrawGizmos.
/// </summary>
public class DbgGizmoFactory : Singleton<DbgGizmoFactory> {
    [Serializable]
    struct VectorGizmo {
        public Vector3 pt;
        public Vector3 dir;
        public float expireTime;
        public VectorGizmo(Vector3 pt, Vector3 dir, float expireTime) {
            this.pt = pt;
            this.dir = dir;
            this.expireTime = expireTime;
        }
    }

    [Header("Vector Gizmo")]
    [SerializeField] float vectorGizmoLifetime = 1f;
    [SerializeField] Color vectorGizmoColor = Color.white;

    readonly List<VectorGizmo> vectorGizmos = new();

    /// <summary>
    /// Draws a temporary vector gizmo.
    /// A sphere is drawn at <paramref name="pt"/> and the arrow extends from that point
    /// in the direction and magnitude of <paramref name="dir"/>.
    /// If <paramref name="dir"/> is zero or nearly zero, only the sphere is drawn using
    /// the zero-direction color.
    /// </summary>
    public static void DrawVectorGizmo(Vector3 pt, Vector3 dir) {
        inst.vectorGizmos.Add(new VectorGizmo(pt, dir, Time.time + inst.vectorGizmoLifetime));
    }

    void OnDrawGizmos() {
        float curTime = Time.time;
        for (int i = vectorGizmos.Count - 1; i >= 0; i--) {
            VectorGizmo gizmo = vectorGizmos[i];
            if (curTime >= gizmo.expireTime) {
                vectorGizmos.RemoveAt(i);
                continue;
            }
            DrawVectorGizmo_Internal(gizmo);
        }
    }

    void DrawVectorGizmo_Internal(VectorGizmo gizmo) {
        DebugUtils.OnDrawGizmos_DrawArrow(gizmo.pt, gizmo.dir, vectorGizmoColor);
    }
}