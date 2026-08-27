using Unity.Collections;

public static class CpInputBuffer {
    public static void BufferInput(
        int id,
        BufferableInput input,
        NativeArray<BufferableInput> bufferedInput,
        NativeArray<float> remainingTime,
        float inputBufferDur
    ) {
        bufferedInput[id] = input;
        remainingTime[id] = inputBufferDur;
    }

    public static void Clear(
        int id,
        NativeArray<BufferableInput> bufferedInput,
        NativeArray<float> remainingTime
    ){
        bufferedInput[id] = BufferableInput.None;
        remainingTime[id] = 0;
    }

    /// <returns>
    /// True if action was in the input buffer and was consumed.
    /// </returns>
    public static bool TryConsumeInput(
        int id,
        BufferableInput input,
        NativeArray<BufferableInput> bufferedInput,
        NativeArray<float> remainingTime

    ){
        if(HasInput(id, input, bufferedInput)){
            Clear(id, bufferedInput, remainingTime);
            return true;
        }
        return false;
    }

    public static bool HasInput(
        int id,
        BufferableInput input,
        NativeArray<BufferableInput> bufferedInput
    )
        => input == bufferedInput[id];
}
