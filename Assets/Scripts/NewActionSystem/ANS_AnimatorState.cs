using System;
using UnityEngine;

/// <summary>
/// Base class for custom animations.
/// Used mainly for events and time ranges. <br/>
/// NOTE: CREATE ONLY ONE ANIMATOR STATE CLASS FOR ONE ANIMATION CLIP! USE THE ANIMATION CLIP NAME AS
/// THE CLASS NAME AND ANIMATOR STATE NAME!<br/>
/// NOTE:Inherited classes should contain all animation events and animation ranges as public read-only auto-implemented properties.
/// </summary>
public abstract class ANS_AnimatorState
{
    public string AnimationName { get; }
    public int AnimationHash { get; }
    public int AnimatorLayer { get; }
    /// <summary>
    /// Last frame index of the animation clip. This has to be set manually since you cannot search animator
    /// states from the animator based on state hash and layer for some reason.
    /// NOTE: Animation frames start at index 0 so the last frame index is  "total frames" - 1.
    /// Use the INDEX of the last frame, and not the total frames of the animation.
    /// </summary>
    public int LastFrameIndex { get; }
    /// <summary>
    /// Crossfade duration in normalized time when transitioning into this state.<br/>
    /// NOTE: If you want to use "fixed duration" e.g. 0.1 second crossfade with 60fps animation:<br/>
    /// 0.1 * 60 = 6,<br/>
    /// then set this property to GeneralUtils.FrameToNormalizedTime(6, LastFrameIndex).
    /// </summary>
    public float NormalizedCrossFadeDurationToThis { get; }


    protected ANS_AnimatorState(
        Animator animator,
        string animationName,
        int animatorLayer,
        int lastFrameIndex,
        float normalizedCrossFadeDurationToThis = 0.1f)
    {
        if (animator == null)
            throw new ArgumentNullException(nameof(animator));

        if (string.IsNullOrEmpty(animationName))
            throw new ArgumentException("Animation name cannot be null or empty.", nameof(animationName));

        if (animatorLayer < 0)
            throw new ArgumentOutOfRangeException(nameof(animatorLayer));

        int stateHash = Animator.StringToHash(animationName);

        if (!animator.HasState(animatorLayer, stateHash))
            throw new ArgumentException(
                $"Animator does not contain state '{animationName}' on layer {animatorLayer}");

        AnimationName = animationName;
        AnimationHash = stateHash;
        AnimatorLayer = animatorLayer;
        LastFrameIndex = lastFrameIndex;
        NormalizedCrossFadeDurationToThis = normalizedCrossFadeDurationToThis;
    }

    /// <summary>
    /// Use this to invoke events and ranges in child classes.
    /// </summary>
    public abstract void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime);

    /// <summary>
    /// Finds the duration of the animation in seconds.<br/>
    /// NOTE: expect the sample rate to be 60 per second. Expects the speed of the animation to be set to 1 in the animator. Expects Time.timeScale to be 1.
    /// </summary>
    public float GetFixedDuration()
    {
        // LastFrameIndex is zero-based, so total frames = LastFrameIndex + 1
        return (LastFrameIndex + 1) / 60f;
    }
}
