using System;

public static class AnimUtils {
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
    public static int FrameAtTime(float timeIntoAnimation, int lastFrameIndex, float samplesPerSecond) {
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
}
