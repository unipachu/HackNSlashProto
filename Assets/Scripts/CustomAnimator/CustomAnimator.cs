using UnityEngine;

/// <summary>
/// Custom animator class which uses Unity's animator for playing animations but handles state transitions fully by itself.<br/>
/// NOTE: Do not use Unity Animator's transitions if you use this class.
/// </summary>
public abstract class CustomAnimator : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    
    protected CustomAnimatorState _initialState;

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

    /// <summary>
    /// Transition to Initial state. Should be called AFTER states have been initialized in the implemented class' Start().
    /// </summary>
    protected virtual void Start()
    {
        // Transition to initial state.
        RequestInstantTransitionTo(_initialState);
    }

    void Update()
    {
        HandleFallbackCheckNTransition();
    }

    private void LateUpdate()
    {
        // Animations have been solved so set the flag back to false.
        // NOTE: You could set this to OnAnimatorMove but that forces the Animator to give the control of the root motion to this script.
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
    /// If the Animator is currently in transition and the current and next states are the same.
    /// </returns>
    public bool IsTransitioningToSameState()
    {
        // NOTE: Currently always uses 0 layer.
        if (!_animator.IsInTransition(0)) return false;

        // NOTE: Currently only works with layer 0.
        AnimatorStateInfo currentState = _animator.GetNextAnimatorStateInfo(0);

        // NOTE: Currently only works with layer 0.
        AnimatorStateInfo nextState = _animator.GetCurrentAnimatorStateInfo(0);

        return currentState.shortNameHash == nextState.shortNameHash;
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

        foreach (CustomAnimatorState animState in animStates)
        {
            if (animState == null)
            {
                Debug.LogError("Null CustomAnimatorState.", this);
                continue;
            }

            string stateName = animState.StateName;

            if (string.IsNullOrWhiteSpace(stateName))
            {
                Debug.LogError("Animator state name is empty.", this);
                continue;
            }

            int stateHash = Animator.StringToHash(stateName);

            // NOTE: Only checks layer 0. Extend if needed.
            Debug.Assert(_animator.HasState(0, stateHash), "Animator didn't have state: " + stateName, this);
            Debug.Assert(GeneralUtils.IsInRange(animState.FallbackTransitionPrecent, 0, 1), "Fallback transition precent was out of range!", this);
            Debug.Assert(!string.IsNullOrEmpty(stateName), "State name was null or empty!", this);
        }
    }
}

// ANIMATOR TESTING AND NOTES:

// - INTERRUPTING A CROSSFADE:
//   When starting a crossfade while the animator is in another crossfade, the animator freezes the current position of the animated object
//   and uses that pose to crossfade to the next animation, while considering the very first animation of the three
//   as the "current animation" still.

// - TRANSITION CALL AND EXECUTION TIME:
//   Newest animator.CrossFade call seems to always override the previous one, if called before Unity's internal animation update.
//   Unity DOES NOT consider an animator to be in a transition (animator.IsInTransition) until that animator's next internal animation
//   update after a transition (e.g. animator.Crossfade()) has been called. There seems to be no way to query the animator if an
//   animator state transition has been buffered before the internal animation update actually starts the transition (which normally
//   happens between Update() and LateUpdate()).

// - TRANSITIONING TO THE SAME ANIMATION:
//   You can transition to the same animator state we are currently in. Crossfades will work as expected, so it seems that the animator
//   state machine instantiates the same state two times.
//   By default it seems that even when transitioning to the same animation, the new animation always starts from the beginning.
//   I.e. it seems that Unity creates two instanses of the same animation and the "next" animation will start from the beginning,
//   while the current animation continues from where its at, and the crossfade is done between these normally.
//   IMPORTANT NOTE: If you call crossfade every frame, the animator will be in the transition FOR EVER, even if it looks like the
//   transition had finished. Make sure not to spam crossfade to the same animation we are currently in.

// - CROSSFADING BETWEEN ROTATIONS:
//   In Animation files, the animation uses Euler angles to interpolate between rotation key frames BY DEFAULT.
//   This works well for limbs that can move in a limited angle range. However when crossfade transition interpolates the rotations between
//   the two animations, it seems to use Quaternion slerp as default AND THIS CANNOT BE CHANGED! This can mean that
//   limbs rotate through a wrong direction (e.g. through a character's body) if that direction is the shortest path
//   between the two interpolated rotations. Only way to prevent this is to make sure that animated limb rotations always stay in +-90 degrees range.

// - WHAT ANIMATOR CONSIDERS AS "CURRENT STATE":
//   When starting a transition to a new state, the new state starts its animation from the beginning. Only after the transition has ended
//   the animator changes the "current state" to reference to the animator state that we just transitioned into. If we start a new transition
//   during a transition, the "current state" during the previous transition will still be considered "current state" during the new 
//   transition.

// - ANIMATION EVENT'S CALLBACK METHOD REFERENCES:
//   Animation events can only reference monobehaviours' methods that are in the same object as the animator component. Also, if
//   multiple components have animation event callback methods that have the same name, the animator might call the wrong method.

// - ANIMATION EVENTS DURING TRANSITIONS:
//   Animation events will trigger in both the animation that we are currently transitioning into and currenty transtitioning out of.
//   This is problematic if you e.g. only want the "next" animation's events to trigger.
//   In code you could check if the Animator is in transition in the animation event callbacks, but if transitioning to the same animation (as
//   the current one), there seems to be no way to check which animator state called the animation event callback since both
//   share the same hash/name.

// - ANIMATION EVENT CALLBACK REFERENCES
//   Changing animation event on one CharacterVisuals object changes it in the others too since the animation event find the first
//   method based on a name stored in the animation event. No matter if the other CharacterVisuals lacks that method, or if it has a
//   different animator controller. This can make reusing animations/animation events for different characters complicated.

// - SKIPPING ANIMATION EVENTS BY OFFSETTING ANIMATION START TIME:
//   If transition offset (start time of the next animation) is used to start transition to animator at a different time than animation
//   start, all the animation events that would've have triggered before the transion offset are (naturally) skipped. This should be taken
//   into consideration if you're using animation events right in the beginning of the animation.

// - OnAnimatorMove() EXECUTION ORDER AND ROOT MOTION CONTROLS:
//   OnAnimatorMove is called for each script attached to an game object with an Animator after Update and before LateUpdate callbacks,
//   after animations for the particular game object's animations have been solved.
//   If your script uses OnAnimatorMove, the Animator will automatically change ApplyRootMotion to "Handled by Script" which prevents
//   root rotation and position changes by the animation curves - you need to apply them through code.

// - ANGULAR ROOT MOTION THROUGH CODE:
//   When applying transform.localRotation *= deltaRot; in OnAnimatorMove to rotate the animated character root, the rotation of the object
//   seems to be offset a little bit every time an animation with root rotation (e.g. knock back or jump attack) are played. For non-imported
//   animated objects you don't have settings to choose which root motions are ignored, which are baked into the animation, and which are
//   applied to the root/controlled by scripts. So for these types of objects it might make sense to create a separate root object for the
//   root motions you want to control through a script and as its child use a "root child" object with motions that control the whole animated
//   character/object, excluding the motions you want to control through root motion/control by a script.
