using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Icludes general helper and utility in the form of general methods and extension methods.
/// </summary>
// TODO MINOR: These methods should probably be categorized better into their own files.
public static class OtherUtils {
    #region =========================================== ANIMATOR HELPERS

    /// <summary>
    /// Find normalized time (0-1) for a frame in an animation, e.g. for firing animation events. <br/>
    /// NOTE: Animation frames start at index 0 so the last frame index is  "total frames" - 1.
    /// Use the INDEX of the last frame, and not the total frames of the animation.
    /// </summary>
    public static float FrameToNormalizedTime(int absoluteFrameIndex, int lastFrameIndex) {
        if (lastFrameIndex <= 0)
            return 0f;
        // Clamp to valid range to avoid overshoot
        absoluteFrameIndex = Math.Clamp(absoluteFrameIndex, 0, lastFrameIndex);
        return absoluteFrameIndex / (float)(lastFrameIndex);
    }

    /// <summary>
    /// Returns the frame index that should be active at a given time (in seconds)
    /// from the start of the animation.
    /// Time is clamped to the animation’s duration, computed from the frame count
    /// and sample rate. The returned frame index is zero-based and clamped to the
    /// range [0, lastFrameIndex].
    /// </summary>
    public static int FrameAtTime(float timeIntoAnimation, int lastFrameIndex, float samplesPerSecond)
    {
        if (lastFrameIndex <= 0 || samplesPerSecond <= 0f)
            return 0;
        float animationLength = (lastFrameIndex + 1) / samplesPerSecond;
        // Clamp time to animation bounds
        timeIntoAnimation = Math.Clamp(timeIntoAnimation, 0f, animationLength);
        // Convert time to continuous frame index
        float frameFloat = timeIntoAnimation * samplesPerSecond;
        // Floor so that time maps to the frame currently being played
        int frameIndex = (int)MathF.Floor(frameFloat);
        // Clamp to valid frame range
        return Math.Clamp(frameIndex, 0, lastFrameIndex);
    }

    /// <summary>
    /// Returns the frame index that should be active when a given amount of time
    /// (in seconds) remains until the end of the animation.
    /// The remaining time is clamped to the animation’s duration. A value of zero
    /// returns the last frame. The returned frame index is zero-based and clamped
    /// to the range [0, lastFrameIndex].
    /// </summary>
    public static int FrameAtTimeUntilAnimationEnd(
        float timeUntilAnimationEnd,
        int lastFrameIndex,
        float samplesPerSecond
    ) {
        if (lastFrameIndex <= 0 || samplesPerSecond <= 0f)
            return 0;
        float animationLength = (lastFrameIndex + 1) / samplesPerSecond;
        // Clamp remaining time to animation bounds
        timeUntilAnimationEnd = Math.Clamp(timeUntilAnimationEnd, 0f, animationLength);
        // How many frames from the end
        float framesFromEnd = timeUntilAnimationEnd * samplesPerSecond;
        int frameIndex = lastFrameIndex - (int)MathF.Floor(framesFromEnd);
        return Math.Clamp(frameIndex, 0, lastFrameIndex);
    }

    /// <summary>
    /// Converts a time value (in seconds) representing how far the animation has
    /// progressed into normalized animation time (0–1).
    /// A value of 0 corresponds to the start of the animation, and 1 corresponds
    /// to the end. The input time is clamped to the animation’s duration, which is
    /// computed from the frame count and sample rate.
    /// </summary>
    public static float TimeIntoAnimationToNormalizedTime(
        float timeIntoAnimation,
        int lastFrameIndex,
        float samplesPerSecond
    ) {
        if (lastFrameIndex < 0 || samplesPerSecond <= 0f)
            return 0f;
        float animationLength = (lastFrameIndex + 1) / samplesPerSecond;
        timeIntoAnimation = Math.Clamp(timeIntoAnimation, 0f, animationLength);
        return timeIntoAnimation / animationLength;
    }

    /// <summary>
    /// Converts a time value (in seconds) representing how much time remains until
    /// the end of the animation into normalized remaining time (0–1).
    /// A value of 1 means the full animation duration remains, and 0 means the
    /// animation has ended. The input time is clamped to the animation’s duration,
    /// which is computed from the frame count and sample rate.
    /// </summary>
    public static float TimeUntilAnimationEndToNormalizedTime(
        float timeUntilAnimationEnd,
        int lastFrameIndex,
        float samplesPerSecond
    ) {
        if (lastFrameIndex < 0 || samplesPerSecond <= 0f)
            return 0f;
        float animationLength = (lastFrameIndex + 1) / samplesPerSecond;
        timeUntilAnimationEnd = Math.Clamp(timeUntilAnimationEnd, 0f, animationLength);
        return timeUntilAnimationEnd / animationLength;
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
        if (condition is null)
            throw new ArgumentNullException(nameof(condition));
        if (pollIntervalMs <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pollIntervalMs),
                "Poll interval must be positive!"
            );
        var waitTask = RunWaitLoop(condition, pollIntervalMs);
        if (timeoutMs < 0) {
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
    private static async Task RunWaitLoop(Func<bool> condition, int pollIntervalMs) {
        while (!condition())
            await Task.Delay(pollIntervalMs).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits until the condition is true.<br/>
    /// NOTE: Default poll interval of 33 ms ~= one frame at 30fps.
    /// </summary>
    /// <returns>
    public static Awaitable WaitUntil(this Func<bool> condition, int pollIntervalMs = 33) {
        if (condition is null)
            throw new ArgumentNullException(nameof(condition));
        if (pollIntervalMs <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pollIntervalMs),
                "Poll interval must be positive!"
            );
        var source = new AwaitableCompletionSource();
        if(condition()) {
            source.SetResult();
            return source.Awaitable;
        }
        var interval = TimeSpan.FromMilliseconds(pollIntervalMs);
        async void Poll() {
            while (!condition()) {
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
    public static Task<T> AsTask<T>(this UnityEvent<T> unityEvent) {
        if (unityEvent == null)
            throw new ArgumentNullException(nameof(unityEvent));
        var tcs = new TaskCompletionSource<T>();
        UnityAction<T> handler = null;
        handler = value => {
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
    public static Task AsTask(this UnityEvent unityEvent) {
        if(unityEvent == null)
            throw new ArgumentNullException(nameof(unityEvent));
        var tcs = new TaskCompletionSource<bool>();
        UnityAction handler = null;
        handler = () => {
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
    public static Awaitable AsAwaitable(this UnityEvent unityEvent) {
        if (unityEvent is null)
            throw new ArgumentNullException(nameof(unityEvent));
        var completionSource = new AwaitableCompletionSource();
        UnityAction handler = null;
        handler = () => {
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
    public static Awaitable<T> AsAwaitable<T>(this UnityEvent<T> unityEvent) {
        if (unityEvent is null)
            throw new ArgumentNullException(nameof(unityEvent));
        var acs = new AwaitableCompletionSource<T>();
        UnityAction<T> handler = null;
        handler = value => {
            unityEvent.RemoveListener(handler);
            acs.TrySetResult(value);
        };
        unityEvent.AddListener(handler);
        return acs.Awaitable;
    }

    #endregion
}

