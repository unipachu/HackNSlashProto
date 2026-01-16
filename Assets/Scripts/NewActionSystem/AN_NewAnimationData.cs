using System;
using UnityEngine;

/// <summary>
/// Base class for CustomAnimationData.
/// Used mainly for events and time ranges. <br/>
/// NOTE: CREATE ONLY ONE ANIMATION DATA FOR ONE ANIMATION! USE THE ANIMATION NAME AS THE CLASS NAME!
/// </summary>
public abstract class AN_NewAnimationData
{

    public string AnimationName { get; }
    public int AnimationHash { get; }
    public int AnimatorLayer { get; }
    /// <summary>
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


    protected AN_NewAnimationData(
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

        int sampleAmount = GeneralUtils.GetNumberOfSamplesStrict(animator, stateHash, animatorLayer);

        // TODO: You could just get the last frame index by using the GetNumberOfSamplesStrict().
        if (lastFrameIndex + 1 != sampleAmount)
            throw new ArgumentException(
                $"Last frame index + 1 = {(lastFrameIndex + 1)} didn't match the amount of samples: {sampleAmount}");

        AnimationName = animationName;
        AnimationHash = stateHash;
        AnimatorLayer = animatorLayer;
        LastFrameIndex = lastFrameIndex;
        NormalizedCrossFadeDurationToThis = normalizedCrossFadeDurationToThis;
    }

}
