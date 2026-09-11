using System;

public static class ArrayUtils{
    /// <summary>
    /// Adds a new element to the end of the array, indicated by <paramref name="usedLength"/>.<br/>
    /// NOTE: Only allocates new array if <paramref name="usedLength"/>, like with a List.<br/>
    /// NOTE 2: You have to keep track of the <paramref name="usedLength"/> outside of this method!
    /// </summary>
    /// <returns>New used length (old + 1).</returns>
    public static int Add<T>(ref T[] array, int usedLength, T element) {
        if (usedLength == array.Length)
            // NOTE: List also doubles the array size when Add forces it to reallocate, and so we do the
            // NOTE C:  same here. Note that the original allocated capacity affects this. (11.9.2026)
            Array.Resize(ref array, usedLength == 0 ? 1 : usedLength * 2);
        array[usedLength] = element;
        return usedLength + 1;
    }

    /// <summary>
    /// Removes element at <paramref name="i"/> and moves elements above it backwards, preserving order.<br/>
    /// NOTE: Does not reallocate array, so <paramref name="usedLength"/> represents "used length"
    /// like in a List.<br/>
    /// NOTE 2: You have to keep track of the <paramref name="usedLength"/> outside of this method!
    /// </summary>
    /// <returns>New used length (old - 1).</returns>
    public static int RemoveAt<T>(T[] array, int usedLength, int i) {
        if (i < usedLength - 1)
            Array.Copy(array, i + 1, array, i, usedLength - i - 1);
        return usedLength - 1;
    }

    /// <summary>
    /// Removes element at <paramref name="i"/> and swaps the last element, indicated by
    /// <paramref name="usedLength"/>, to its place.
    /// NOTE: Does not reallocate array, so <paramref name="usedLength"/> represents "used length"
    /// like in a List.<br/>
    /// NOTE 2: You have to keep track of the <paramref name="usedLength"/> outside of this method!
    /// </summary>
    /// <returns>New used length (old - 1).</returns>
    public static int RemoveAtSwapBack<T>(T[] array, int usedLength, int i) {
        int lastIndex = usedLength - 1;
        if (i != lastIndex)
            array[i] = array[lastIndex];
        return usedLength - 1;
    }
}
