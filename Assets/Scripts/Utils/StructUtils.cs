using System;
using Unity.Collections;
using UnityEngine;

public static class StructUtils {
    /// <summary>
    /// Helper for allocating native arrays. So much boilerplate...
    /// </summary>
    public static NativeArray<T> Alloc<T>(int capacity) where T : unmanaged {
        return new NativeArray<T>(capacity, Allocator.Persistent);
    }

    /// <summary>
    /// Can be used in e.g. switch expressions for the default return type.
    /// </summary>
    public static TReturn LogErrorForInput<TInput, TReturn>(TInput input) {
        Debug.LogError($"Switch defaulted with {input}");
        return default;
    }
}
