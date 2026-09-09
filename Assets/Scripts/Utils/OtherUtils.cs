using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Icludes general helper and utility in the form of general methods and extension methods.<br/>
/// NOTE: These should be probably categorized in their own classes... (9.9.2026)
/// </summary>
public static class OtherUtils {
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

