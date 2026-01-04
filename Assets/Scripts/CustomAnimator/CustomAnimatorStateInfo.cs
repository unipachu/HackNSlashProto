using System;
using UnityEngine;

/// <summary>
/// Info about an animation state used by the CustomAnimator class.
/// </summary>
// TODO: Make this into an abstract class and each animator state an inherited class. Also create on enter and on exit states.
[Serializable]
public class CustomAnimatorStateInfo
{
    public readonly string ThisAnimationName;
    public readonly int ThisAnimationHash;

    public readonly float CrossFadeDurationToThis;

    /// <summary>
    /// Normalized time [0-1] when we should transition
    /// to the next queued animation (if any).
    /// </summary>
    public readonly float QueueTransitionPercent;

    /// <summary>
    /// Animation to play if queue is empty. Set this to null if you don't want to transition to a fallback animation
    /// and instead want to keep playing this animation.
    /// </summary>
    public readonly CustomAnimatorStateInfo FallbackAnimation;

    /// <summary>
    /// Normalized time [0-1] when we should transition
    /// to the fallback animation if queue is empty.
    /// </summary>
    public readonly float FallbackTransitionPercent;


    public CustomAnimatorStateInfo(
        string thisAnimationName,
        float crossFadeDurationToThis = 0.1f,
        float queueTransitionPercent = 0,
        CustomAnimatorStateInfo fallbackAnimation = null,
        float fallbackTransitionPercent = 0.9f
        )
    {
        ThisAnimationName = thisAnimationName;
        ThisAnimationHash = Animator.StringToHash(thisAnimationName);
        CrossFadeDurationToThis = crossFadeDurationToThis;
        QueueTransitionPercent = queueTransitionPercent;
        FallbackTransitionPercent = fallbackTransitionPercent;
        FallbackAnimation = fallbackAnimation;

        Debug.Assert(QueueTransitionPercent >= 0 && QueueTransitionPercent <= 1, "Transition precent was out of range.");
        Debug.Assert(FallbackTransitionPercent >= 0 && FallbackTransitionPercent <= 1, "Transition precent was out of range.");
    }
}
