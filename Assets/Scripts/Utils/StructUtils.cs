using Unity.Collections;

public static class StructUtils {
    /// <summary>
    /// Helper for allocating native arrays. So much boilerplate...
    /// </summary>
    public static NativeArray<T> Alloc<T>(int capacity) where T : unmanaged {
        return new NativeArray<T>(capacity, Allocator.Persistent);
    }
}
