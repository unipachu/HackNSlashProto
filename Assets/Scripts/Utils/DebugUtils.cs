using UnityEditor;
using UnityEngine;

/// <summary>
/// Utility methods for debugging.
/// </summary>
public class DebugUtils : MonoBehaviour {
    /// <summary>
    /// Default gizmo color if no override is provided.
    /// </summary>
    public static Color DefaultColor = Color.white;

    static Mesh arrowHeadMesh;

    public static void OnDrawGizmos_DrawArrow(Vector3 startPt, Vector3 arrowVec, Color? color = null) {
        Color prevColor = Gizmos.color;
        Color arrowCol = color ?? DefaultColor;
        const float startSphereRad = 0.05f;
        if (arrowVec.IsZeroOrNearlyZero()) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startPt, startSphereRad);
            Gizmos.color = prevColor;
            return;
        }
        Vector3 normArrowVec = arrowVec.normalized;
        Vector3 arrowTip = startPt + arrowVec;
        float arrowHeadLen = Mathf.Min(0.075f, arrowVec.magnitude * 0.25f);
        float arrowHeadRad = arrowHeadLen * 0.5f;
        Vector3 arrowHeadBasePt = arrowTip - normArrowVec * arrowHeadLen;
        Color arrowHeadCol = new Color(
            arrowCol.r * 0.8f,
            arrowCol.g * 0.8f,
            arrowCol.b * 0.8f,
            arrowCol.a
        );
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startPt, startSphereRad);
        Gizmos.color = arrowCol;
        Gizmos.DrawLine(startPt, arrowHeadBasePt);
        if (arrowHeadMesh == null) {
            arrowHeadMesh = new Mesh();
            arrowHeadMesh.name = "DbgGizmoArrowHead";
            arrowHeadMesh.vertices = new[] {
            // Side 0
            new Vector3(-1f, 0f, -1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3(0f, 1f, 0f),
            // Side 1
            new Vector3(-1f, 0f, 1f),
            new Vector3(1f, 0f, 1f),
            new Vector3(0f, 1f, 0f),
            // Side 2
            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 0f, -1f),
            new Vector3(0f, 1f, 0f),
            // Side 3
            new Vector3(1f, 0f, -1f),
            new Vector3(-1f, 0f, -1f),
            new Vector3(0f, 1f, 0f),
            // Bottom
            new Vector3(-1f, 0f, -1f),
            new Vector3(1f, 0f, -1f),
            new Vector3(1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f)
        };
            arrowHeadMesh.triangles = new[] {
            0, 1, 2,
            3, 4, 5,
            6, 7, 8,
            9, 10, 11,
            12, 13, 14,
            12, 14, 15
        };
            arrowHeadMesh.RecalculateNormals();
        }
        Quaternion arrowHeadRot = Quaternion.FromToRotation(Vector3.up, normArrowVec);
        Gizmos.color = arrowHeadCol;
        Gizmos.DrawMesh(
            arrowHeadMesh,
            arrowHeadBasePt,
            arrowHeadRot,
            new Vector3(arrowHeadRad, arrowHeadLen, arrowHeadRad)
        );
        Gizmos.color = prevColor;
    }

    /// <summary>
    /// Draws a wireframe capsule between the given points with the specified radius.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void OnDrawGizmos_DrawCapsule(
        Vector3 pt0,
        Vector3 pt1,
        float radius,
        Color? color = null
    ) {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawWireSphere(pt0, radius);
        Gizmos.DrawWireSphere(pt1, radius);
        Vector3 axis = pt1 - pt0;
        float axisLength = axis.magnitude;
        if (axisLength > 0.0001f) {
            axis /= axisLength;
            Vector3 side = Vector3.Cross(axis, Vector3.up);
            if (side.sqrMagnitude < 0.0001f)
                side = Vector3.Cross(axis, Vector3.right);
            side.Normalize();
            Vector3 up = Vector3.Cross(axis, side);
            Vector3 sideOffset = side * radius;
            Vector3 upOffset = up * radius;
            Gizmos.DrawLine(pt0 + sideOffset, pt1 + sideOffset);
            Gizmos.DrawLine(pt0 - sideOffset, pt1 - sideOffset);
            Gizmos.DrawLine(pt0 + upOffset, pt1 + upOffset);
            Gizmos.DrawLine(pt0 - upOffset, pt1 - upOffset);
        }
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a wireframe sphere at the given position with the specified radius.
    /// ? after Color parameter type means that Color struct is allowed to be null.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void OnDrawGizmos_DrawSphere(Vector3 center, float radius, Color? color = null) {
        Color previousColor = Gizmos.color;
        // Applies color based on wheter the parameter color was null or not.
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawSphere(center, radius);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a wireframe sphere at the given position with the specified radius.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void OnDrawGizmos_DrawWireSphere(Vector3 center, float radius, Color? color = null) {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawWireSphere(center, radius);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a line between the two specified points.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void OnDrawGizmos_DrawLine(Vector3 start, Vector3 end, Color? color = null) {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawLine(start, end);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a solid cube at the given position with the specified size.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void OnDrawGizmos_DrawCube(Vector3 center, Vector3 size, Color? color = null) {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawCube(center, size);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a wireframe cube at the given position with the specified size.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void OnDrawGizmos_DrawWireCube(Vector3 center, Vector3 size, Color? color = null) {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws text labels in Scene View. Call this in OnDrawGizmos or other methods that are run in the editor to make the labels appear.
    /// Is set to do nothing in builds, since Handles.Label is an editor-only function and would cause errors in builds. 
    /// </summary>
    public static void DrawLabel(Vector3 position, string text, Color color, int fontSize = 12, TextAnchor alignment = TextAnchor.MiddleCenter, FontStyle fontStyle = FontStyle.Bold, bool wordWrap = true, bool richText = false) {
#if UNITY_EDITOR
        GUIStyle labelStyle = new GUIStyle {
            normal = new GUIStyleState { textColor = color },
            alignment = alignment,
            fontStyle = fontStyle,
            fontSize = fontSize,
            wordWrap = wordWrap,
            richText = richText
        };
        Handles.Label(position, text, labelStyle);
#endif
    }
}
