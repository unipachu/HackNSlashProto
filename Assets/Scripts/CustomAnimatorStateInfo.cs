using System;
using UnityEngine;

/// <summary>
/// Info about an animation state used by the CustomAnimator class.
/// </summary>
[Serializable]
public class CustomAnimatorStateInfo
{
    public readonly float CrossFadeDurationToThis;

    public readonly string ThisAnimationName;
    public readonly int ThisAnimationHash;

    /// <summary>
    /// Normalized time [0–1] when we should transition
    /// to the next queued animation (if any).
    /// </summary>
    public readonly float QueueTransitionPercent;

    /// <summary>
    /// Animation to play if queue is empty. Set this to null if you don't want to transition to a fallback animation
    /// and instead want to keep playing this animation.
    /// </summary>
    public readonly CustomAnimatorStateInfo FallbackAnimation;

    /// <summary>
    /// Normalized time [0–1] when we should transition
    /// to the fallback animation if queue is empty.
    /// </summary>
    public readonly float FallbackTransitionPercent;


    public CustomAnimatorStateInfo(
        float crossFadeDurationToThis,
        string thisAnimationName,
        float queueTransitionPercent,
        CustomAnimatorStateInfo fallbackAnimation = null,
        float fallbackTransitionPercent = 0
        )
    {
        CrossFadeDurationToThis = crossFadeDurationToThis;
        ThisAnimationName = thisAnimationName;
        ThisAnimationHash = Animator.StringToHash(thisAnimationName);
        QueueTransitionPercent = queueTransitionPercent;
        FallbackTransitionPercent = fallbackTransitionPercent;
        FallbackAnimation = fallbackAnimation;

        Debug.Assert(QueueTransitionPercent > 0 && QueueTransitionPercent <= 1, "Transition precent was out of range.");
        Debug.Assert(FallbackTransitionPercent > 0 && FallbackTransitionPercent <= 1, "Transition precent was out of range.");
    }
}
