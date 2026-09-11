// TODO: Rename to ...Utils
public static class CpInputBuffer {
    public static void BufferInput(
        ref BufferableInput bufferedInput,
        ref float remainingTime,
        BufferableInput inputToBuffer,
        float inputBufferDur
    ) {
        bufferedInput = inputToBuffer;
        remainingTime = inputBufferDur;
    }

    public static void Clear(
        ref BufferableInput bufferedInput,
        ref float remainingTime
    ){
        bufferedInput = BufferableInput.None;
        remainingTime = 0;
    }

    /// <returns>
    /// True if action was in the input buffer and was consumed.
    /// </returns>
    public static bool TryConsumeInput(
        BufferableInput input,
        ref BufferableInput bufferedInput,
        ref float remainingTime

    ){
        if(input == bufferedInput){
            Clear(ref bufferedInput, ref remainingTime);
            return true;
        }
        return false;
    }
}
