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
