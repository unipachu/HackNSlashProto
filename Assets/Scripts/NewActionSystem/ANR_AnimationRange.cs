using System;

/// <summary>
/// Represents a normalized time range (0–1) during an animation.
/// </summary>
// TODO: Maybe you should remove the conditions 
public class ANR_AnimationRange
{
    public Action RangeActiveAction;

    /// <summary>
    /// Normalized time (0–1) at which the range starts.<br/>
    /// NOTE: For an easier time, you can use GeneralUtils.FrameToNormalizedTime() TODO: Is this correct?
    /// </summary>
    public float NormalizedRangeStart { get; }

    /// <summary>
    /// Normalized time (0–1) at which the range ends.
    /// NOTE: For an easier time, you can use GeneralUtils.FrameToNormalizedTime() TODO: Is this correct?
    /// </summary>
    public float NormalizedRangeEnd { get; }

    /// <summary>
    /// If the range is fully skipped within a single frame, should it still be considered entered at least once?
    /// </summary>
    public bool EnterRangeOnceIfCompletelyJumpedOverThisFrame { get; }

    /// <summary>
    /// Count the frame when crossing the end of the range as being 'in range'. Overrides 
    /// <see cref="EnterRangeOnceIfCompletelyJumpedOverThisFrame"/> if set to true. <br/>
    /// NOTE: Imo the best way to decide whether to use this or not is that higher frame rate should be more advantageous to the PLAYER,
    /// so that player can't "cheat" by having a low framerate, but the advantage from a higher framerate should scale logarithmically,
    /// so that the benefit diminishes as the frame rate approaches target frame rate (e.g. 60). E.g. for enemy attack hitboxes this should be
    /// set to 'true' and for the player's attack's hitboxes this should be set to 'false'.
    /// </summary>
    public bool CountFrameWhenCrossedOverRangeEnd { get; }


    /// <summary>
    /// If transition offset caused to jump over this range on the first frame, do we consider range entered on the first frame?
    /// </summary>
    public bool ConsiderFrameEnteredDuringFirstFrameIfSkippedAtStart { get; }

    public ANR_AnimationRange(
        float normalizedRangeStart,
        float normalizedRangeEnd,
        bool countFrameWhenCrossedOverRangeEnd = false,
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
        CountFrameWhenCrossedOverRangeEnd = countFrameWhenCrossedOverRangeEnd;
        EnterRangeOnceIfCompletelyJumpedOverThisFrame = enterRangeAtLeastOnce;
    }

    /// <returns>
    /// True if action was invoked, otherwise false.
    /// </returns>
    public bool InvokeActionIfInRange(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
        if (IsInRange(activeStateClampedNormalizedTime, previousActiveStateClampedNormalizedTime))
        {
            RangeActiveAction?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Assuming the current state holds this range, are we in range?<br/>
    /// NOTE: Action will not be invoked unless this method is called e.g. in
    /// the <see cref="ANL_AnimatorLayer.UpdateLayer()"/> method.
    /// </summary>
    // TODO: If the animation loops around, this does not work. You need to use non-clamped time.
    public bool IsInRange(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
        bool isInRange = activeStateClampedNormalizedTime.IsInRange(
            NormalizedRangeStart,
            NormalizedRangeEnd);

        // If completely jumped over the range during this frame.
        if (EnterRangeOnceIfCompletelyJumpedOverThisFrame
            && previousActiveStateClampedNormalizedTime < NormalizedRangeStart
            && activeStateClampedNormalizedTime > NormalizedRangeEnd)
        {
            isInRange = true;
        }

        // If crossed over the range end this frame
        if (CountFrameWhenCrossedOverRangeEnd
            && previousActiveStateClampedNormalizedTime <= NormalizedRangeEnd
            && activeStateClampedNormalizedTime > NormalizedRangeEnd)
        {
            isInRange = true;
        }

        if(ConsiderFrameEnteredDuringFirstFrameIfSkippedAtStart)
        {
            // TODO:
        }

        return isInRange;
    }
}