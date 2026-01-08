using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Side
{
    Left,
    Right,
}

public enum Directions2DVertical
{
    Left,
    Right,
    Up,
    Down,
}

public enum Directions2DHorizontal
{
    Left,
    Right,
    Forward,
    Backward,
}

public enum Directions3D
{
    Left,
    Right,
    Up,
    Down,
    Forward,
    Backward,
}

public static class GeneralUtils
{
    /// <summary>
    /// Default gizmo color if no override is provided.
    /// </summary>
    public static Color DefaultColor = Color.white;


    /// <summary>
    /// Normalizes angle to 0-360 degrees.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float Normalize360(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    public static bool IsInRange(float x, float greaterThanOrEqualTo, float lessThanOrEqualTo)
    {
        return x >= greaterThanOrEqualTo && x <= lessThanOrEqualTo;
    }

    /// <summary>
    /// Checks if the two positions are within the allowed distance from each other.
    /// </summary>
    /// <returns></returns>
    public static bool IsWithinAllowedDist(Vector3 a, Vector3 b, float minDist, float maxDist)
    {
        float distance = Vector3.Distance(a, b);
        return distance >= minDist && distance <= maxDist;
    }

    // TODO: Perhaps expand this so that parameter takes in the axis instead of the transform. Also write the direction of the rotation.
    /// <summary>
    /// Returns world pos and rot of an object when rotated around the right-axis of a pivot object. 
    /// </summary>
    public static (Vector3, Quaternion) ComputeNewPosRotByRotationAroundPivotXAxis(Transform movedObject, Transform pivotObject, float rotationAroundAxis, float rotationMult = 1)
    {
        // NOTE: rotationMult is used here to rotate the object slightly further.
        float deltaXAngle = rotationAroundAxis * rotationMult;

        //Debug.Log("delta x angle: " + deltaXAngle);

        Quaternion deltaRotAroundPivotRight = Quaternion.AngleAxis(deltaXAngle, pivotObject.right);
        Vector3 movedObjectPosInPivotSpace = Quaternion.Inverse(pivotObject.rotation) * (movedObject.position - pivotObject.position);
        Quaternion movedObjectRotInPivotSpace = Quaternion.Inverse(pivotObject.rotation) * movedObject.rotation;

        Quaternion pivotFutureRot = deltaRotAroundPivotRight * pivotObject.rotation;
        Vector3 movedObjectNextPos = pivotObject.position + pivotFutureRot * movedObjectPosInPivotSpace;
        Quaternion movedObjectNextRot = pivotFutureRot * movedObjectRotInPivotSpace;

        return (movedObjectNextPos, movedObjectNextRot);
    }

    /// <summary>
    /// Transforms point from rigidbody's local space to world space using rb.position.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.TransformPoint.
    /// </summary>
    public static Vector3 RigidbodyUnscaledTransformPoint(Rigidbody rb, Vector3 pointInRbSpace)
    {
        return rb.rotation * pointInRbSpace + rb.position;
    }

    /// <summary>
    /// Transforms point from world space to rb's local space using rb.position.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.InverseTransformPoint.
    /// </summary>
    public static Vector3 RigidbodyUnscaledInverseTransformPoint(Rigidbody rb, Vector3 pointInWorldSpace)
    {
        return Quaternion.Inverse(rb.rotation) * (pointInWorldSpace - rb.position);
    }

    /// <summary>
    /// Converts a world space rotation into the rigidbody's local space rotation.
    /// </summary>
    public static Quaternion RotationFromWorldToRbSpace(Rigidbody rb, Quaternion rotationInWorldSpace)
    {
        return Quaternion.Inverse(rb.rotation) * rotationInWorldSpace;
    }

    /// <summary>
    /// Converts a rigidbody's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion RotationFromRbSpaceToWorld(Rigidbody rb, Quaternion rotationInRbSpace)
    {
        return rb.rotation * rotationInRbSpace;
    }

    /// <summary>
    /// Transforms point from transform's local space to world space.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.TransformPoint.
    /// </summary>
    public static Vector3 UnscaledTransformPoint(Transform transform, Vector3 pointInTransformSpace)
    {
        return transform.rotation * pointInTransformSpace + transform.position;
    }

    /// <summary>
    /// Transforms point from world space to transform's local space.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.InverseTransformPoint.
    /// </summary>
    public static Vector3 UnscaledInverseTransformPoint(Transform transform, Vector3 pointInWorldSpace)
    {
        return Quaternion.Inverse(transform.rotation) * (pointInWorldSpace - transform.position);
    }

    /// <summary>
    /// Converts a world space rotation into the transform's local space rotation.
    /// </summary>
    public static Quaternion RotationFromWorldToTransformSpace(Transform transform, Quaternion rotationInWorldSpace)
    {
        return Quaternion.Inverse(transform.rotation) * rotationInWorldSpace;
    }

    /// <summary>
    /// Converts a transforms's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion RotationFromTransformSpaceToWorld(Transform transform, Quaternion rotationInRbSpace)
    {
        return transform.rotation * rotationInRbSpace;
    }

    /// <summary>
    /// Flashes the given mesh renderer between two materials for a specified duration.
    /// </summary>
    public static IEnumerator FlashMeshCoroutine(
        MeshRenderer meshRenderer,
        Material materialA,
        Material materialB,
        float flashDuration = 1.5f,
        float flashInterval = 0.2f,
        Action onComplete = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            // Switch materials based on the flash interval.
            meshRenderer.material = (elapsedTime % flashInterval < flashInterval / 2)
                ? materialA
                : materialB;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Invoke the callback if provided.
        onComplete?.Invoke();
    }

    /// <summary>
    /// Flashes mesh between two materials based on Time.time. Has to be called each frame to work.
    /// </summary>
    /// <param name="meshRenderer"></param>
    /// <param name="materialA"></param>
    /// <param name="materialB"></param>
    /// <param name="flashInterval"></param>
    public static void FlashMeshUpdate(MeshRenderer meshRenderer, Material materialA, Material materialB, float flashInterval = 0.2f)
    {
        // Switch materials based on the flash interval.
        meshRenderer.material = (Time.time % flashInterval < flashInterval / 2)
            ? materialA
            : materialB;
    }


    /// <summary>
    /// When wrap mode of image's sprite is repeat, this will cause a horizontal scrolling effect when called from Update.
    /// </summary>
    public static void ScrollImage(RawImage image, float scrollSpeed)
    {
        image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime, 0), image.uvRect.size);
    }

    /// <summary>
    /// When wrap mode of image's sprite is repeat, this will cause a horizontal scrolling effect when called from Update.
    /// </summary>
    public static void ScrollImages(RawImage[] images, float scrollSpeed)
    {
        foreach (RawImage image in images)
        {
            image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime, 0), image.uvRect.size);
        }
    }

    public static void VerticalSineMovement(RectTransform rectTransform, Vector3 startPos, float speed, float amplitude)
    {
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = startPos + new Vector3(0f, yOffset, 0f);
    }

    /// <summary>
    /// Draws a wireframe sphere at the given position with the specified radius.
    /// ? after Color parameter type means that Color struct is allowed to be null.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void DrawSphere(Vector3 center, float radius, Color? color = null)
    {
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
    public static void DrawWireSphere(Vector3 center, float radius, Color? color = null)
    {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawWireSphere(center, radius);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a line between the two specified points.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void DrawLine(Vector3 start, Vector3 end, Color? color = null)
    {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawLine(start, end);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a solid cube at the given position with the specified size.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void DrawCube(Vector3 center, Vector3 size, Color? color = null)
    {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawCube(center, size);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws a wireframe cube at the given position with the specified size.
    /// Call this in MonoBehaviour's OnDrawGizmos or OnDrawGizmosSelected methods.
    /// </summary>
    public static void DrawWireCube(Vector3 center, Vector3 size, Color? color = null)
    {
        Color previousColor = Gizmos.color;
        Gizmos.color = color ?? DefaultColor;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = previousColor;
    }

    /// <summary>
    /// Draws text labels in Scene View. Call this in OnDrawGizmos or other methods that are run in the editor to make the labels appear.
    /// Is set to do nothing in builds, since Handles.Label is an editor-only function and would cause errors in builds. 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public static void DrawLabel(Vector3 position, string text, Color color, int fontSize = 12, TextAnchor alignment = TextAnchor.MiddleCenter, FontStyle fontStyle = FontStyle.Bold, bool wordWrap = true, bool richText = false)
    {
#if UNITY_EDITOR
        GUIStyle labelStyle = new GUIStyle
        {
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
