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
    /// This custom animator never changes state until the very end of its Update(). Other scripts may request change in animator state,
    /// which is saved here to wait for the end of the Update().
    /// </summary>
    private CustomAnimatorState _requestedState;

    /// <summary>
    /// Currently active state (animation). If in transition, refers to the "next animator state".
    /// </summary>
    public CustomAnimatorState ActiveState => _activeState;

    void Update()
    {
        // NOTE: Animator's initial state will be its own default state. It will transition to 
        if (_activeState == null)
        {
            InstantTransitionToAnimation(_initialState);
            // NOTE: needs to return here, so that the animatior can transition to the initial animation after this Update().
            return;
        }

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
        Debug.Assert(IsActiveState(0, _activeState.StateHash),
            "Active state of the custom animator was different than the active state of the Animator."
            + AnimatorStateInfo(),
            this);

        // Transition to fallback animation
        if (_activeState.FallbackState != null
            && normalizedTime >= _activeState.FallbackTransitionPrecent)
        {
            //Debug.Log("Normalized time was: " + normalizedTime
            //    + ", and so it was time for " + _state.StateName + " to fallback to: " + _state.FallbackState.StateName);

            // TODO: Start the next animation from a later point based on how much the normalized time is over the fallback precent.
            // TODO CONTD: Be wary though that this might skip some animator events (I don't know if it does).
            CrossFadeInFixedTimeToAnimation(_activeState.FallbackState);
        }

        // TODO: This is here because Unity's Animator component does not start transition until the internal animation update that happens
        // TODO CONTD: in between Update and LateUpdate, so the "current state" of the Animator lags behind this, causing problems.
        // TODO: This is problematic since if this update is called before other updates where new animation are requested,
        // TODO CONTD: changes to animations will be one frame behind.
        // TODO CONTD: And if you run this in LateUpdate, it will also skip the animation update of this frame (since animation update happens
        // TODO CONTD: in between Update and LateUpdate).
        // TODO CONTD: This is just a temporary solution. I might need to check out Unity Playables...
        if(_requestedState != null)
        {
            StartCrossfadeTo(_requestedState);
            _requestedState = null;
        }
    }

    /// <summary>
    /// Starts crossfade to another animator state.
    /// </summary>
    /// <param name="startFromBeginning">
    /// If the same animation is currently playing, should this start it from the beginning (instead of letting it play
    /// from where it currently is)? True by default.
    /// </param>
    private void StartCrossfadeTo(CustomAnimatorState newAnimation, bool startFromBeginning = true)
    {
        // NOTE: Currently only works with layer 0.
        if (IsActiveState(0, newAnimation.StateHash) && !startFromBeginning) return;
        CrossFadeInFixedTimeToAnimation(newAnimation);
    }

    public void RequestCrossfadeTo(CustomAnimatorState newAnimation)
    {
        if(_requestedState != null)
        {
            Debug.LogWarning("During this animation update cycle, " +
                "another state was already requested for " + gameObject.name + ": " + _requestedState.StateName, this);
        }
        _requestedState = newAnimation;
    }

    /// <summary>
    /// Uses CrossFadeInFixedTime. <br/>
    /// NOTE: Always call this when you want to transition to new animation. NEVER crossfade or play other animations through any other method.
    /// </summary>
    private void CrossFadeInFixedTimeToAnimation(CustomAnimatorState nextAnimation, float fixedTimeOffset = 0)
    {
        //Debug.Log("Transitioning to state: " + nextAnimation.StateName);

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
    private void InstantTransitionToAnimation(CustomAnimatorState nextAnimation, float normalizedTimeOffset = 0)
    {
        //Debug.Log("Transitioning to state: " + nextAnimation.StateName);

        _activeState = nextAnimation;
        // NOTE: Currently only works with layer 0.
        _animator.Play(
            nextAnimation.StateHash,
            0,
            normalizedTimeOffset
        );
    }

    /// <returns>
    /// The hash of the current animation state if not in transition, or the the hash of the next animation state if in transition.
    /// </returns>
    /// 
    protected int HashOfActiveAnimation(int animatorLayer)
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
    /// True if currently in the specified state (if not in transition) or transitioning into the specified
    /// state (if in transition) in the Animator.
    /// </returns>
    public bool IsActiveState(int animatorLayer, int stateHash)
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

// - TODO: TEST: If transition offset to new state "skips" the time of an animation event, will it still fire? Will animation events during
//   a transition fire, on both the current and next animations? You might want to prevent animation events if they are triggered in the
//   "current" animation during a transition.
