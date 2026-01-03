using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom animator class which uses Unity's animator for playing animations but handles state transitions fully by itself.
/// Has the ability to enqueue animations. <br/>
/// NOTE: Do not use Unity Animator's transitions if you use this class.
/// </summary>
public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private readonly Queue<CustomAnimatorStateInfo> _animationQueue = new();

    /// <summary>
    /// Animation that is currently played.
    /// NOTE: This offends the SSOT principle, since CustomAnimationInfo saves the hash of the animation separately 
    /// NOTE CONTD: from the Unity's Animator for transition purposes.
    /// NOTE CONTD: Use HashOfActiveAnimation() to get the current animation hash, to get the hash the actual animator is using.
    /// </summary>
    private CustomAnimatorStateInfo _currentAnimInfo;

    void Update()
    {
        // NOTE: currentAnim variable holds the animation we are currently in (if not transitioning) or
        // the animation we are currently transitioning into. We want the corresponsing state from the animator.
        AnimatorStateInfo animatorState;
        // NOTE: Currently always uses 0 layer.
        if (_animator.IsInTransition(0))
        {
            // NOTE: Currently always uses 0 layer.
            animatorState = _animator.GetNextAnimatorStateInfo(0);
        }
        else
        {
            // NOTE: Currently only works with layer 0.
            animatorState = _animator.GetCurrentAnimatorStateInfo(0);
        }
        float normalizedTime = animatorState.normalizedTime;

        Debug.Assert(!IsInOrIsTransitioningToAnimatorState(_animator, 0, _currentAnimInfo.ThisAnimationHash),
            "currentAnim was different than the active state in the animator.");

        // If animation queue is empty -> transition to fallback animation or keep playing current animation.
        if (_animationQueue.Count == 0
            && _currentAnimInfo.FallbackAnimation != null
            && normalizedTime >= _currentAnimInfo.FallbackTransitionPercent)
        {
            // TODO: Start the next animation from a later point based on how much the normalized time is over the fallback precent.
            TransitionToAnimation(_currentAnimInfo.FallbackAnimation);
        }
        // If there are animations in the queueue:
        else
        {
            // NOTE: There's a risk that the animation will never transition if the current animation is looping and the
            // NOTE CONTD: normalized time jumps over the QueueTransitionPercent and back to the beginning of the animation in one frame.
            if (normalizedTime >= _currentAnimInfo.QueueTransitionPercent)
            {
                // TODO: Start the next animation from a later point based on how much the normalized time is over the fallback precent.
                // TODO CONTD: NOTE: This might cause unintentional jumps in animations if a new animation is added to the queue
                // TODO CONTD: after QueueTransitionPercent point has passed. Therefor you should make it optional or remove such functionality
                // TODO CONTD: from animations that loop/don't have fallback animation.
                // TODO CONTD: Also note that if the normalized time passed 1, then the normalized time will show incorrectly how much time
                // has passed since QueueTransitionPercent.
                TransitionToAnimation(_animationQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// Adds an animation to the animation queue. The animation will play after a time specified by the animation playing just before this.
    /// </summary>
    public void EnqueueAnimation(CustomAnimatorStateInfo animation)
    {
        _animationQueue.Enqueue(animation);
    }

    /// <summary>
    /// Clears current animation queue and starts playing this animation instantly (without adding it into the queue).
    /// </summary>
    public void InterruptAnimationQueue(CustomAnimatorStateInfo newAnimation)
    {
        _animationQueue.Clear();
        TransitionToAnimation(newAnimation);
    }

    public int AnimationQueueCount() => _animationQueue.Count;

    /// <summary>
    /// Animation queue can be safely cleared at any point.
    /// </summary>
    public void ClearAnimationQueue()
    {
        _animationQueue.Clear();
    }

    /// <summary>
    /// Uses CrossFadeInFixedTime. <br/>
    /// NOTE: Always call this when you want to transition to new animation. NEVER crossfade or play other animations through any other method.
    /// </summary>
    private void TransitionToAnimation(CustomAnimatorStateInfo nextAnimation, float fixedTimeOffset = 0)
    {
        _currentAnimInfo = nextAnimation;
        // NOTE: Currently only works with layer 0.
        _animator.CrossFadeInFixedTime(
            nextAnimation.ThisAnimationHash,
            nextAnimation.CrossFadeDurationToThis,
            0,
            fixedTimeOffset
        );
    }

    /// <returns>
    /// The hash of the current animation state if not in transition, or the the hash of the next animation state if in transition.
    /// </returns>
    /// 
    public int HashOfActiveAnimation(int animatorLayer)
    {
        if (_animator.IsInTransition(animatorLayer))
        {
            return _animator.GetCurrentAnimatorStateInfo(animatorLayer).shortNameHash;
        }
        else
        {
            return _animator.GetNextAnimatorStateInfo(animatorLayer).shortNameHash;
        }
    }

    /// <returns>
    /// True if currently in the specified state or transitioning into the specified state.
    /// </returns>
    // TODO: Make non static and remove animator parameter.
    public static bool IsInOrIsTransitioningToAnimatorState(Animator animator, int animatorLayer, int stateHash)
    {
        AnimatorStateInfo current =
            animator.GetCurrentAnimatorStateInfo(animatorLayer);

        if (current.shortNameHash == stateHash)
            return true;

        if (animator.IsInTransition(animatorLayer))
        {
            AnimatorStateInfo next =
                animator.GetNextAnimatorStateInfo(animatorLayer);

            if (next.shortNameHash == stateHash)
                return true;
        }

        return false;
    }
}
