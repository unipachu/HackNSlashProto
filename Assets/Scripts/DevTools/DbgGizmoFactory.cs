using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory for temporary debug gizmos.
/// Gizmos are added through static methods and rendered by this component's OnDrawGizmos.
/// </summary>
public class DbgGizmoFactory : Singleton<DbgGizmoFactory> {
    [Header("Vector Gizmo")]
    [SerializeField] bool drawVectorGizmos = true;
    [SerializeField] float vectorGizmoLifetime = 2f;
    [SerializeField] Color vectorGizmoColor = Color.white;

    readonly List<VectorGizmo> vectorGizmos = new();

    void OnDrawGizmos() {
        float curTime = Time.time;
        if(drawVectorGizmos)
            for (int i = vectorGizmos.Count - 1; i >= 0; i--) {
                VectorGizmo gizmo = vectorGizmos[i];
                if (curTime >= gizmo.expireTime) {
                    vectorGizmos.RemoveAt(i);
                    continue;
                }
                DebugUtils.OnDrawGizmos_DrawArrow(gizmo.pt, gizmo.dir, vectorGizmoColor);
            }
    }

    /// <summary>
    /// Draws a temporary vector gizmo.
    /// A sphere is drawn at <paramref name="startPt"/> and the arrow extends from that point
    /// in the direction and magnitude of <paramref name="vec"/>.
    /// If <paramref name="vec"/> is zero or nearly zero, only the sphere is drawn using
    /// the zero-direction color.
    /// </summary>
    public static void DrawVectorGizmo(Vector3 startPt, Vector3 vec) {
        inst.vectorGizmos.Add(new VectorGizmo(startPt, vec, Time.time + inst.vectorGizmoLifetime));
    }

    /// <summary>
    /// Draws temporary vector gizmos for all provided hit points and normals.
    /// </summary>
    public static void DrawVectorGizmos(Vector3[] startPt, Vector3[] vec, int arrayUsedLength) {
        for (int i = 0; i < arrayUsedLength; i++) {
            DrawVectorGizmo(startPt[i], vec[i]);
        }
    }
}