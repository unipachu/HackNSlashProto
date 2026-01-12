using UnityEngine;

/// <summary>
/// Custom animator class which uses Unity's animator for playing animations but handles state transitions fully by itself.
/// Has the ability to enqueue animations. <br/>
/// NOTE: Do not use Unity Animator's transitions if you use this class.
/// NOTE: Crossfades now use fixed time which does not scale if the animator speed is scaled.
/// </summary>
public abstract class CustomAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CustomAnimatorState _initialState;

    /// <summary>
    /// Currently active state (animation). If in transition, refers to the "next animator state".
    /// NOTE: Animator component refers to its own current/next state -> this offends the SSOT principle. 
    /// </summary>
    private CustomAnimatorState _activeState;
   
    /// <summary>
    /// This is used to prevent fallback transition checker from trying to change to fallback animation based on Animator's active animation
    /// when it doesn't match this scripts active state (since they don't match if animation transition was requested this frame).
    /// </summary>
    private bool _requestedStateChangeThisFrame = false;

    /// <summary>
    /// Currently active state (animation). If in transition, refers to the "next animator state".
    /// </summary>
    public CustomAnimatorState ActiveState => _activeState;

    protected virtual void Start()
    {
        // Transition to initial state.
        RequestInstantTransitionTo(_initialState);
    }

    void Update()
    {
        HandleFallbackCheckNTransition();
    }

    private void OnAnimatorMove()
    {
        // Animations have been solved so set the flag back to false.
        _requestedStateChangeThisFrame = false;
    }

    /// <summary>
    /// NOTE: Prevents transition to fallback animations if another transition was requested this frame. This is to
    /// NOTE C: prevent problems caused by mismatch of this script's "active state" and the Animator's "active state" since the 
    /// NOTE C: Animator only returns the requested new animation after Unity has entered into internal animation update.
    /// </summary>
    private void HandleFallbackCheckNTransition()
    {
        // If transition to next state has been requested, 
        if (_requestedStateChangeThisFrame) return;

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

        // Transition to fallback animation
        if (_activeState.FallbackState != null
            && normalizedTime >= _activeState.FallbackTransitionPrecent
            && !_requestedStateChangeThisFrame
            )
        {
            //Debug.Log("Normalized time was: " + normalizedTime
            //    + ", and so it was time for " + _state.StateName + " to fallback to: " + _state.FallbackState.StateName);

            RequestFixedTimeCrossfadeTo(_activeState.FallbackState);
        }
    }

    /// <summary>
    /// Call this to ask for a new animation.
    /// </summary>
    public void RequestFixedTimeCrossfadeTo(CustomAnimatorState nextAnimation, float fixedTimeOffset = 0, bool startFromBeginning = true)
    {
        _requestedStateChangeThisFrame = true;

        //Debug.Log("Transitioning to state: " + nextAnimation.StateName);

        // NOTE: Currently only works with layer 0.
        if (IsActiveState(nextAnimation) && !startFromBeginning) return;

        _activeState = nextAnimation;
        // NOTE: Currently only works with layer 0.
        _animator.CrossFadeInFixedTime(
            nextAnimation.StateHash,
            nextAnimation.CrossFadeDurationToThis,
            0,
            fixedTimeOffset
        );
    }

    /// <summary>
    /// Uses CrossFadeInFixedTime. <br/>
    /// NOTE: Always call this when you want to transition to new animation. NEVER crossfade or play other animations through any other method.
    /// </summary>
    private void RequestInstantTransitionTo(CustomAnimatorState nextAnimation, float normalizedTimeOffset = 0, bool startFromBeginning = true)
    {
        _requestedStateChangeThisFrame = true;

        //Debug.Log("Transitioning to state: " + nextAnimation.StateName);

        // NOTE: Currently only works with layer 0.
        if (IsActiveState(nextAnimation) && !startFromBeginning) return;

        _activeState = nextAnimation;
        // NOTE: Currently only works with layer 0.
        _animator.Play(
            nextAnimation.StateHash,
            0,
            normalizedTimeOffset
        );
    }

    /// <returns>
    /// NOTE: Checks the current state of the CustomAnimator which might differ from the Animator's state.
    /// </returns>
    public bool IsActiveState(CustomAnimatorState state)
    {
        return _activeState == state;
    }

    /// <returns>
    /// Debug info about states of the animator.
    /// </returns>
    protected string AnimatorStateInfo()
    {
        if (_activeState == null) return "CustomAnimator's state was null.";
        string returnString = "";
        returnString += "Current state of the CustomAnimator: " + _activeState.StateName
                + " , going by hash: " + _activeState.StateHash
                + ". Current state of the animator was: " + _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (_animator.IsInTransition(0))
        {
            returnString += ". Next state of the animator was: " + _animator.GetNextAnimatorStateInfo(0).shortNameHash;

        }
        return returnString;
    }

    /// <summary>
    /// Checks that all provided animation states exist in the animator.
    /// </summary>
    protected void ValidateAnimationStates(params CustomAnimatorState[] animStates)
    {
        if (animStates == null || animStates.Length == 0)
        {
            Debug.LogWarning("ValidateAnimationStates called with no states.", this);
            return;
        }

        foreach (var animState in animStates)
        {
            if (animState == null)
            {
                Debug.LogError("Null CustomAnimatorState.", this);
                continue;
            }

            string stateName = animState.StateName;

            if (string.IsNullOrWhiteSpace(stateName))
            {
                Debug.LogError("Animator state name is empty.", animState);
                continue;
            }

            int stateHash = Animator.StringToHash(stateName);

            // NOTE: Only checks layer 0. Extend if needed.
            Debug.Assert(_animator.HasState(0, stateHash), "Animator didn't have state: " + stateName, animState);
        }
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

// - Animation events can only reference monobehaviours' methods that are in the same object as the animator component. Also, if
//   multiple component have animation event callback methods that have the same name, the animator might call the wrong method.

// - ANIMATION EVENTS will trigger in both the animation that we are currently transitioning into and currenty transtitioning out of.
//   This is problematic if you e.g. only want the "next" animation's events to trigger.

// - If transition offset (start time of the next animation) is used to start transition to animator at a different time than animation
//   start, all the animation events that would've have triggered before the transion offset are (naturally) skipped. You could 
//   check if the Animator is in transition in the animation event callbacks, but if transitioning to the same animation as before,
//   it might be hard to separate between the current and next animation callbacks (if you want the "next" animation event callbacks to
//   trigger but not the "current" animation event callbacks).

// - OnAnimatorMove is called for each script attached to an game object with an Animator after Update and before LateUpdate callbacks,
//   after animations for the particular game object's animations have been solved.
