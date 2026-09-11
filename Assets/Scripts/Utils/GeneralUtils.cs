using Unity.Collections;
using UnityEngine;

/// <summary>
/// Utils for commonly used and generic types.
/// </summary>
public static class GeneralUtils {
    /// <summary>
    /// Helper for allocating native arrays. So much boilerplate...
    /// </summary>
    public static NativeArray<T> AllocArray<T>(int capacity) where T : unmanaged
        => new NativeArray<T>(capacity, Allocator.Persistent);

    /// <summary>
    /// Helper for allocating native arrays. So much boilerplate...
    /// </summary>
    public static NativeList<T> AllocList<T>(int capacity) where T : unmanaged
        => new NativeList<T>(capacity, Allocator.Persistent);

    /// <summary>
    /// Can be used in e.g. switch expressions to log error and return default type.
    /// </summary>
    public static TReturn LogErrorForInput<TInput, TReturn>(TInput input) {
        Debug.LogError($"Input caused error: {input}");
        return default;
    }
}
