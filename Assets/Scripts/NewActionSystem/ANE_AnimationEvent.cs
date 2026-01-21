using System;

/// <summary>
/// Used with the custom animator system. Replaces animation events.
/// Represents a single time-based event within an animation.
/// </summary>
public class ANE_AnimationEvent
{
    public event Action AnimationEventAction;

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

    protected ANE_AnimationEvent(
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

    /// <summary>
    /// Invokes the action of this animation event if conditions are met.<br/>
    /// NOTE: Action will not be invoked unless this method is called e.g. in
    /// the <see cref="ANL_AnimatorLayer.UpdateLayer()"/> method.
    /// </summary>
    /// <returns>
    /// True if Action was invoked, false otherwise.
    /// </returns>
    public bool InvokeActionIfConditionsForTriggeringEventMet(float currentStateTime, float previousStateTime)
    {
        if(ShouldTriggerThisFrame(currentStateTime, previousStateTime))
        {
            AnimationEventAction?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Assuming the currently active state holds this trigger, are conditions for this trigger met?
    /// NOTE: Calling this does not invoke the related action.
    /// </summary>
    // TODO: Make sure that the animation actions are invoked as many times as the animation has looped over them (if the animation
    // TODO C: is set to be looping), maybe create a fallback though, e.g. max 100 invokes and then throw an error or warning).
    // TODO C: This requires you to use non-clamped time instead.
    public bool ShouldTriggerThisFrame(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
        bool shouldTrigger = NormalizedEventTime > previousActiveStateClampedNormalizedTime
            && NormalizedEventTime < activeStateClampedNormalizedTime;

        if(TriggerOnSubsequentLoops)
        {
            // TODO: 
        }

        if (TriggerIfSkippedAtStart)
        {
            //TODO: Check if this was the first frame into the state and if so, if the initial time offset caused to jump over
            //TODO C: the event, consider event triggered anyway.
        }

        return shouldTrigger;
    }
}