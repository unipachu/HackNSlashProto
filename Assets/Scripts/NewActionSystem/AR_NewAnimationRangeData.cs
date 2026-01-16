using System;

/// <summary>
/// Represents a normalized time range (0–1) during an animation.
/// </summary>
public class AR_NewAnimationRangeData
{
    /// <summary>
    /// Normalized time (0–1) at which the range starts.
    /// </summary>
    public float NormalizedRangeStart { get; }

    /// <summary>
    /// Normalized time (0–1) at which the range ends.
    /// </summary>
    public float NormalizedRangeEnd { get; }

    /// <summary>
    /// If the range is fully skipped within a single frame,
    /// should it still be considered entered at least once.
    /// </summary>
    public bool EnterRangeAtLeastOnce { get; }

    public AR_NewAnimationRangeData(
        float normalizedRangeStart,
        float normalizedRangeEnd,
        bool enterRangeAtLeastOnce = false)
    {
        normalizedRangeStart.ValidateNormalizedValue(nameof(normalizedRangeStart));
        normalizedRangeEnd.ValidateNormalizedValue(nameof(normalizedRangeEnd));

        if (normalizedRangeStart >= normalizedRangeEnd)
        {
            throw new ArgumentException(
                "NormalizedRangeStart must be less than NormalizedRangeEnd.");
        }

        NormalizedRangeStart = normalizedRangeStart;
        NormalizedRangeEnd = normalizedRangeEnd;
        EnterRangeAtLeastOnce = enterRangeAtLeastOnce;
    }
}