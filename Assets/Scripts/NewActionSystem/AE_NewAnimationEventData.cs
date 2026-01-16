using System;

/// <summary>
/// Used with the custom animator system. Replaces animation events.
/// Represents a single time-based event within an animation.
/// </summary>
public class AE_NewAnimationEventData
{
    /// <summary>
    /// Normalized time (0–1) at which the event should trigger.
    /// </summary>
    public float NormalizedEventTime { get; }

    /// <summary>
    /// If the animation is looping, should the event trigger on every loop
    /// instead of only the first playthrough?
    /// </summary>
    public bool TriggerOnSubsequentLoops { get; }

    /// <summary>
    /// Should the event trigger when entering animator state if the animation started at a normalized time
    /// later than the event time?
    /// </summary>
    public bool TriggerIfSkippedAtStart { get; }

    protected AE_NewAnimationEventData(
        float normalizedEventTime,
        bool triggerOnSubsequentLoops = true,
        bool triggerIfSkippedAtStart = true)
    {
        if (normalizedEventTime < 0f || normalizedEventTime > 1f)
            throw new ArgumentOutOfRangeException(
                nameof(normalizedEventTime),
                "Normalized event time must be between 0 and 1.");

        NormalizedEventTime = normalizedEventTime;
        TriggerOnSubsequentLoops = triggerOnSubsequentLoops;
        TriggerIfSkippedAtStart = triggerIfSkippedAtStart;
    }
}