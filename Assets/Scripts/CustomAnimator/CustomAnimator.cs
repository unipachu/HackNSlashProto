using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom animator class which uses Unity's animator for playing animations but handles state transitions fully by itself.
/// Has the ability to enqueue animations. <br/>
/// NOTE: Do not use Unity Animator's transitions if you use this class.
/// </summary>
// TODO: The animation queue system is stupid. Just use animation events.
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
        // NOTE: If no animation has been initialized through this class, the update will return.
        // TODO: You might want to create a "InitialAnimState" variable or similar.
        if (_currentAnimInfo == null) return;

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

        // NOTE: The assert will fail if the an animator state transition is called before Unity's internal animation update
        // NOTE CONTD: has the time to start the transition. The differenece between this class' current state hash and
        // NOTE CONTD: the Unity's animator's current state hash can cause problems later. The animator's event calls also cause assert
        // NOTE CONTD: to fail, because the transition is only applied during the next frame.
        // TODO: Figure a solution to this problem. Maybe buffer animation transitions and only apply them after this
        // TODO CONTD: assert (and other related logic).
        //Debug.Assert(IsInOrIsTransitioningToAnimatorState(0, _currentAnimInfo.ThisAnimationHash),
        //    "currentAnim was different than the active state in the animator. "
        //    + AnimatorStateInfo(),
        //    this);

        // Transition to fallback animation
        if (_animationQueue.Count == 0
            && _currentAnimInfo.FallbackAnimation != null
            && normalizedTime >= _currentAnimInfo.FallbackTransitionPercent)
        {
            // TODO: Start the next animation from a later point based on how much the normalized time is over the fallback precent.
            TransitionToAnimation(_currentAnimInfo.FallbackAnimation);
        }
        // Transition to the next animation in the queue:
        // NOTE: There's a risk that the animation will never transition if the current animation is looping and the
        // NOTE CONTD: normalized time jumps over the QueueTransitionPercent and back to the beginning of the animation in one frame.
        else if (_animationQueue.Count != 0 && normalizedTime >= _currentAnimInfo.QueueTransitionPercent)
        {
            // TODO: Start the next animation from a later point based on how much the normalized time is over the fallback precent.
            // TODO CONTD: NOTE: This might cause unintentional jumps in animations if a new animation is added to the queue
            // TODO CONTD: after QueueTransitionPercent point has passed. Therefor you should make it optional or remove such functionality
            // TODO CONTD: from animations that loop/don't have fallback animation.
            // TODO CONTD: Also note that if the normalized time passed 1, then the normalized time will show incorrectly how much time
            // TODO CONTD: has passed since QueueTransitionPercent.
            // TODO CONTD: Actually it might be possible that normalized time actually goes beyond 1 and Unity only uses fractional part
            // TODO CONTD: for looping.
            TransitionToNextAnimationInQueue();
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
    /// Instantly start transition to the next animation in the queue. This can be used if e.g. want an animation event to trigger
    /// the next animation in the queue.
    /// </summary>
    public void TransitionToNextAnimationInQueue()
    {
        TransitionToAnimation(_animationQueue.Dequeue());
    }

    /// <summary>
    /// Clears current animation queue and starts playing this animation instantly (without adding it into the queue).
    /// </summary>
    /// <param name="startFromBeginning">
    /// If the same animation is currently playing, should this start it from the beginning (instead of letting it play
    /// from where it currently is)? True by default.
    /// </param>
    public void InterruptAnimationQueue(CustomAnimatorStateInfo newAnimation, bool startFromBeginning = true)
    {
        ClearAnimationQueue();
        // NOTE: Currently only works with layer 0.
        if (IsInOrIsTransitioningToAnimatorState(0, newAnimation.ThisAnimationHash) && !startFromBeginning) return;
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
    /// True if currently in the specified state (if not in transition) or transitioning into the specified state (if in transition).
    /// </returns>
    public bool IsInOrIsTransitioningToAnimatorState(int animatorLayer, int stateHash)
    {
        if (_animator.IsInTransition(animatorLayer))
        {
            AnimatorStateInfo next =
                _animator.GetNextAnimatorStateInfo(animatorLayer);

            //Debug.Log("next hash: " + next.shortNameHash + ". saved hash: " + stateHash);
            if (next.shortNameHash == stateHash)
                return true;
        }
        else
        {
            AnimatorStateInfo current =
            _animator.GetCurrentAnimatorStateInfo(animatorLayer);

            //Debug.Log("current hash: " + current.shortNameHash + ". saved hash: " + stateHash);
            if (current.shortNameHash == stateHash)
                return true;
        }

        return false;
    }

    /// <returns>
    /// If the Animator used by this CustomAnimator has the specified state.
    /// </returns>
    public bool HasState(int animatorLayer, int stateHash)
    {
        return _animator.HasState(animatorLayer, stateHash);
    }

    /// <returns>
    /// Debug info about states of the animator.
    /// </returns>
    public string AnimatorStateInfo()
    {
        if (_currentAnimInfo == null) return "CustomAnimator's state was null.";
        string returnString = "";
        returnString += "Current state of the CustomAnimator: " + _currentAnimInfo.ThisAnimationName
                + " , going by hash: " + _currentAnimInfo.ThisAnimationHash
                + ". Current state of the animator was: " + _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (_animator.IsInTransition(0))
        {
            returnString += ". Next state of the animator was: " + _animator.GetNextAnimatorStateInfo(0).shortNameHash;

        }
        return returnString;
    }
}

// ANIMATOR TESTING AND NOTES:

// - PROBLEM: It seems like transition times are visibly longer than 0.5 seconds even though the transition time was set to 0.5f.

// - When starting crossfade during another crossfade, the animator seems to know how to smoothly cross
//   fade from the in-transition pose to the third one.

// - Newest animator.CrossFade call seems to always override the previous one, no matter if they have been called the same frame or not.

// - Animator never seems to reach the next animation. This might be caused by the fact that I call cross fade transition every frame.
//   INDEED - if crossfade is called during an earlier unfinished transition, the transition is started again, though now it is transitioned
//   from the pose it was during the two transitions. I'm not sure how Unity does these transitions from transitions but this doesn't seem
//   to cause a memory leak or any other problems. This proves that you can transition to the same animation we are currently in.
//   IMPORTANT NOTE: If you call crossfade every frame, the animator will be in the transition FOR EVER, even if it looks like the
//   transition had finished.
//   When looking at the animator window in Unity, it seems that when a transition is interrupted, Unity pauses the first animation,
//   and uses that paused pose to transition to the next animation (though the next animation will still play during transition).

// - It also seems that when transition is not interrupted by another transition, the transition will be completely smooth,
//   whereas if you enter a transition while already in a transition, there seems to be a "jump" in the bun's" animation.
//   This weird jump in the bun's animation seems to also happen when crossfading from walk to Test animation.
//   No wait, this seems to happen also when transitioning in between test animations.
//   I don't understand why this jump happens. Could it be related to the fact that the animation uses euler angles and jumps from 360 to 0?
//   Probably.

// To animate rotations properly, you should probably create a custom method which reveals up and forward vectors to the animator and
// and uses LookRotation to rotate the object. Or make sure no animated game object "rotates over +-180 degrees.
// NO WAIT: I changed the bun rotations so that they stay in the range -120 to 120 degrees and the jumps still happen. What?
// The body of the character seems to rotate smoothly so I have no idea what's going on.
// Perhaps the animator tries to interpolate between angles by taking the shortest path, but since the shortest path changes to the other side
// because the animation can rotate the object over 180 angle, the object jumps. I'll try to set the buns rotation between -89 to 89
// in the animation.
// YES, that seems to have fixed the problem.

// - When starting a transition to a new state, the new state starts its animation from the beginning. So after the transition has ended and
//   the animator changes the "current state", the animation actually has started playing before and might have already finished if
//   the animation is shorted than the transition.
//   This might cause problem if you expect the animator's "current animation" be the same one that just finished playing.

// - By default it seems that even when transitioning to the same animation, the new animation always starts from the beginning.
//   I.e. it seems like Unity creates two instanses of the same animation and the "next" animation will start from the beginning,
//   while the current one continues from where its at, and the crossfade is done between these normally.
