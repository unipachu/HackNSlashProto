using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;

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

/// <summary>
/// Icludes general helper and utility in the form of general methods and extension methods.
/// </summary>
public static class GeneralUtils
{
    /// <summary>
    /// Default gizmo color if no override is provided.
    /// </summary>
    public static Color DefaultColor = Color.white;

    #region =========================================== MATH AND TRANSFORM AND RIGIDBODY TRANSFORMATIONS

    /// <summary>
    /// Normalizes angle to 0-360 degrees.
    /// </summary>
    public static float Normalize360(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    public static bool IsInRange(float value, float greaterThanOrEqualTo, float lessThanOrEqualTo)
    {
        return value >= greaterThanOrEqualTo && value <= lessThanOrEqualTo;
    }

    /// <summary>
    /// Checks if the two positions are within the allowed distance from each other.
    /// </summary>
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
    public static Vector3 RigidbodyUnscaledTransformPoint(this Rigidbody rb, Vector3 pointInRbSpace)
    {
        return rb.rotation * pointInRbSpace + rb.position;
    }

    /// <summary>
    /// Transforms point from world space to rb's local space using rb.position.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.InverseTransformPoint.
    /// </summary>
    public static Vector3 RigidbodyUnscaledInverseTransformPoint(this Rigidbody rb, Vector3 pointInWorldSpace)
    {
        return Quaternion.Inverse(rb.rotation) * (pointInWorldSpace - rb.position);
    }

    /// <summary>
    /// Converts a world space rotation into the rigidbody's local space rotation.
    /// </summary>
    public static Quaternion RotationFromWorldToRbSpace(this Rigidbody rb, Quaternion rotationInWorldSpace)
    {
        return Quaternion.Inverse(rb.rotation) * rotationInWorldSpace;
    }

    /// <summary>
    /// Converts a rigidbody's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion RotationFromRbSpaceToWorld(this Rigidbody rb, Quaternion rotationInRbSpace)
    {
        return rb.rotation * rotationInRbSpace;
    }

    /// <summary>
    /// Transforms point from transform's local space to world space.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.TransformPoint.
    /// </summary>
    public static Vector3 UnscaledTransformPoint(this Transform transform, Vector3 pointInTransformSpace)
    {
        return transform.rotation * pointInTransformSpace + transform.position;
    }

    /// <summary>
    /// Transforms point from world space to transform's local space.
    /// Does not scale the point, in other words: ignores transform.localScale unlike transform.InverseTransformPoint.
    /// </summary>
    public static Vector3 UnscaledInverseTransformPoint(this Transform transform, Vector3 pointInWorldSpace)
    {
        return Quaternion.Inverse(transform.rotation) * (pointInWorldSpace - transform.position);
    }

    /// <summary>
    /// Converts a world space rotation into the transform's local space rotation.
    /// </summary>
    public static Quaternion RotationFromWorldToTransformSpace(this Transform transform, Quaternion rotationInWorldSpace)
    {
        return Quaternion.Inverse(transform.rotation) * rotationInWorldSpace;
    }

    /// <summary>
    /// Converts a transforms's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion RotationFromTransformSpaceToWorld(this Transform transform, Quaternion rotationInRbSpace)
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

    /// <returns>
    /// Value mapped from range 1 to range 2.
    /// </returns>
    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, value));
    }

    #endregion
    #region =========================================== COLLECTION UTILS

    /// <summary>
    /// Copies a block from this array to another. Using a span is often faster than using an array copy.
    /// </summary>
    public static void BlockCopy<T>(this Span<T> src, int srcOffset, Span<T> dst, int dstOffset, int count)
    {
        if ((uint)(srcOffset + count) > (uint)src.Length)
            throw new ArgumentException("Source span is to small.");
        if ((uint)(dstOffset + count) > (uint)dst.Length)
            throw new ArgumentException("Destination span is to small.");

        src.Slice(srcOffset, count).CopyTo(dst.Slice(dstOffset));
    }

    /// <summary>
    /// Copies a block from this array to another.
    /// </summary>
    public static void BlockCopy<T>(this T[] src, int srcOffset, T[] dst, int dstOffset, int count)
    {
        if(src == null) throw new ArgumentNullException(nameof(src));
        if (dst == null) throw new ArgumentNullException(nameof(dst));

        src.AsSpan().BlockCopy(srcOffset, dst.AsSpan(), dstOffset, count);
    }

    #endregion
    #region =========================================== MESH / MATERIAL EFFECTS

    /// <summary>
    /// Flashes mesh between two materials based on Time.time. Has to be called each frame to work.
    /// </summary>
    public static void FlashMeshUpdate(this MeshRenderer meshRenderer, Material materialA, Material materialB, float flashInterval = 0.2f)
    {
        // Switch materials based on the flash interval.
        meshRenderer.material = (Time.time % flashInterval < flashInterval / 2)
            ? materialA
            : materialB;
    }

    #endregion
    #region =========================================== 2D VISUALS AND UI EFFECTS

    /// <summary>
    /// When wrap mode of image's sprite is repeat, this will cause a horizontal scrolling effect when called from Update.
    /// </summary>
    public static void ScrollImage(this RawImage image, float scrollSpeed)
    {
        image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime, 0), image.uvRect.size);
    }

    /// <summary>
    /// When wrap mode of image's sprite is repeat, this will cause a horizontal scrolling effect when called from Update.
    /// </summary>
    public static void ScrollImages(this RawImage[] images, float scrollSpeed)
    {
        foreach (RawImage image in images)
        {
            image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime, 0), image.uvRect.size);
        }
    }

    public static void VerticalSineMovement(this RectTransform rectTransform, Vector3 startPos, float speed, float amplitude)
    {
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = startPos + new Vector3(0f, yOffset, 0f);
    }

    #endregion
    #region =========================================== ANIMATOR HELPERS

    /// <returns>
    /// The hash of the current animator state if not in transition, or the the hash of the next animator state if in transition.
    /// </returns>
    public static int HashOfActiveAnimatorState(this Animator animator, int animatorLayer)
    {
        if (animator.IsInTransition(animatorLayer))
        {
            return animator.GetCurrentAnimatorStateInfo(animatorLayer).shortNameHash;
        }
        else
        {
            return animator.GetNextAnimatorStateInfo(animatorLayer).shortNameHash;
        }
    }

    /// <returns>
    /// True if currently in the specified state (if not in transition) or transitioning into the specified
    /// state (if in transition) in the Animator.
    /// </returns>
    public static bool IsActiveAnimatorState(this Animator animator, int animatorLayer, int stateHash)
    {
        if (animator.IsInTransition(animatorLayer))
        {
            AnimatorStateInfo next =
                animator.GetNextAnimatorStateInfo(animatorLayer);

            //Debug.Log("next hash: " + next.shortNameHash + ". saved hash: " + stateHash);
            if (next.shortNameHash == stateHash)
                return true;
        }
        else
        {
            AnimatorStateInfo current =
            animator.GetCurrentAnimatorStateInfo(animatorLayer);

            //Debug.Log("current hash: " + current.shortNameHash + ". saved hash: " + stateHash);
            if (current.shortNameHash == stateHash)
                return true;
        }

        return false;
    }

    #endregion
    #region =========================================== TASK / AWAITABLE / UNITY EVENT EXTENSIONS

    /// <summary>
    /// Waits until the condition is true.<br/>
    /// NOTE: Default poll interval of 33 ms ~= one frame at 30fps.
    /// </summary>
    /// <returns>
    /// False if timeouted (amd timeoutMs was set to >0), otherwise true.
    /// </returns>

    public static async Task<bool> WaitUntil(this Func<bool> condition, int timeoutMs = -1, int pollIntervalMs = 33)
    {
        if (condition is null) throw new ArgumentNullException(nameof(condition));
        if (pollIntervalMs <= 0) throw new ArgumentOutOfRangeException(nameof(pollIntervalMs), "Poll interval must be positive!");

        var waitTask = RunWaitLoop(condition, pollIntervalMs);

        if (timeoutMs < 0)
        {
            await waitTask;
            return true;
        }

        var timeoutTask = Task.Delay(timeoutMs);
        var finished = await Task.WhenAny(waitTask, timeoutTask);
        return finished == waitTask;
    }

    /// <summary>
    /// Helper for WaitUntil();
    /// </summary>
    private static async Task RunWaitLoop(Func<bool> condition, int pollIntervalMs)
    {
        while (!condition())
            await Task.Delay(pollIntervalMs).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits until the condition is true.<br/>
    /// NOTE: Default poll interval of 33 ms ~= one frame at 30fps.
    /// </summary>
    /// <returns>
    public static Awaitable WaitUntil(this Func<bool> condition, int pollIntervalMs = 33)
    {
        if (condition is null) throw new ArgumentNullException(nameof(condition));
        if (pollIntervalMs <= 0) throw new ArgumentOutOfRangeException(nameof(pollIntervalMs), "Poll interval must be positive!");

        var source = new AwaitableCompletionSource();

        if(condition())
        {
            source.SetResult();
            return source.Awaitable;
        }

        var interval = TimeSpan.FromMilliseconds(pollIntervalMs);

        async void Poll()
        {
            while (!condition())
            {
                await Awaitable.WaitForSecondsAsync((float)interval.TotalSeconds);
            }
            source.SetResult();
        }

        Poll();
        return source.Awaitable;
    }

    /// <summary>
    /// Converts a <see cref="UnityEvent{T}"/> into a <see cref="Task{T}"/> that completes
    /// the next time the event is invoked. The event listener is automatically removed
    /// after the first invocation.
    /// </summary>
    public static Task<T> AsTask<T>(this UnityEvent<T> unityEvent)
    {
        if (unityEvent == null) throw new ArgumentNullException(nameof(unityEvent));

        var tcs = new TaskCompletionSource<T>();
        UnityAction<T> handler = null;
        handler = value =>
        {
            unityEvent.RemoveListener(handler);
            tcs.TrySetResult(value);
        };

        unityEvent.AddListener(handler);
        return tcs.Task;
    }

    /// <summary>
    /// Converts a <see cref="UnityEvent"/> into a <see cref="Task"/> that completes
    /// the next time the event is invoked. The event listener is automatically removed
    /// after the first invocation.
    /// </summary>
    public static Task AsTask(this UnityEvent unityEvent)
    {
        if(unityEvent == null) throw new ArgumentNullException(nameof(unityEvent));

        var tcs = new TaskCompletionSource<bool>();
        UnityAction handler = null;
        handler = () =>
        {
            unityEvent.RemoveListener(handler);
            tcs.TrySetResult(true);
        };

        unityEvent.AddListener(handler);
        return tcs.Task;
    }

    /// <summary>
    /// Converts a <see cref="UnityEvent"/> into an <see cref="Awaitable"/> that completes
    /// the next time the event is invoked. The event listener is automatically removed
    /// after the first invocation.
    /// </summary>
    public static Awaitable AsAwaitable(this UnityEvent unityEvent)
    {
        if (unityEvent is null) throw new ArgumentNullException(nameof(unityEvent));

        var completionSource = new AwaitableCompletionSource();
        UnityAction handler = null;
        handler = () =>
        {
            unityEvent.RemoveListener(handler);
            completionSource.TrySetResult();
        };

        unityEvent.AddListener(handler);
        return completionSource.Awaitable;
    }

    /// <summary>
    /// Converts a <see cref="UnityEvent{T}"/> into an <see cref="Awaitable{T}"/> that completes
    /// the next time the event is invoked, yielding the event argument as the result.
    /// The event listener is automatically removed after the first invocation.
    /// </summary>
    public static Awaitable<T> AsAwaitable<T>(this UnityEvent<T> unityEvent)
    {
        if (unityEvent is null) throw new ArgumentNullException(nameof(unityEvent));

        var acs = new AwaitableCompletionSource<T>();
        UnityAction<T> handler = null;
        handler = value =>
        {
            unityEvent.RemoveListener(handler);
            acs.TrySetResult(value);
        };

        unityEvent.AddListener(handler);
        return acs.Awaitable;
    }

    #endregion
    #region =========================================== DEBUGGING / EDITOR-ONLY

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
    #endregion

}

