using System;
using UnityEngine;



/// <summary>
/// Used to control the animations of the CharacterVisuals prefab.
/// </summary>
// TODO: animator.CrossFadeInFixedTime doesn't scale with animator.speed.
// TODO CONTD: This might cause problems in some cases and therefore you might want to use animator.CrossFade instead.
public class CharacterVisualsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // NOTE: Hashed animator state names. These NEED to match the ones in the animator. Make sure to assert these.
    // TODO: Make a dictionary to find what hash refers to what state string name.
    public static readonly int _animHash_Idle = Animator.StringToHash("CharacterVisuals_Idle");
    public static readonly int _animHash_Walk = Animator.StringToHash("CharacterVisuals_Walk");
    public static readonly int _animHash_KnockBackBackward = Animator.StringToHash("CharacterVisuals_KnockBack_Backward");
    public static readonly int _animHash_Test1 = Animator.StringToHash("CharacterVisuals_Test1");
    public static readonly int _animHash_Test2 = Animator.StringToHash("CharacterVisuals_Test2");
    public static readonly int _animHash_Test3 = Animator.StringToHash("CharacterVisuals_Test3");

    private int currentStateHash;
    private int nextStateHash;
    private float nextStateCrossFadeDuration;
    private bool waitingForEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Assert every hashed animation state here!
        Debug.Assert(_animator.HasState(0, _animHash_Idle), "Animator state didn't exist!");
        Debug.Assert(_animator.HasState(0, _animHash_Walk), "Animator state didn't exist!");
        Debug.Assert(_animator.HasState(0, _animHash_KnockBackBackward), "Animator state didn't exist!");
    }

    void Update()
    {
        if (waitingForEnd)
        {
            // NOTE: Expects animations to be on layer 0.
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            Debug.Assert(stateInfo.shortNameHash == currentStateHash, "Current state was not the same one set in the PlayNext method.");
            Debug.Assert(!stateInfo.loop, "State cannot be looping, because then transition to the next state will never happen.");

            // NOTE: Current animation cannot be looping, since the transition happens only after the animation has "finished" i.e. normlized time is >= 1.
            if (stateInfo.normalizedTime >= 1f)
            {
                waitingForEnd = false;
                _animator.CrossFadeInFixedTime(nextStateHash, nextStateCrossFadeDuration, 0 , 0);
            }
        }
    }

    /// <summary>
    /// Plays the first animation and after it finishes, goes to play the second one <br/>
    /// NOTE: The first animation needs to be non looping currently, TODO: make it so you 
    /// can set when the transition stats e.g. 90% into the animation.
    /// </summary>
    // TODO: Try this: Animation.CrossFadeQueued or do a better queuing system for the animations.
    // TODO: Make it clear in summary or name that this uses fixed time crossfade.

    private void PlayThen(int firstAnimationHash, float firstCrossFadeTime, int secondAnimationHash, float secondCrossFadeTime)
    {
        currentStateHash = firstAnimationHash;
        nextStateHash = secondAnimationHash;
        nextStateCrossFadeDuration = secondCrossFadeTime;
        waitingForEnd = true;
        // TODO: Animator seems to not be able to crossfade into the same state it already was in. What to do?
        Debug.Log("Went to play then!");
        
        // If already in the 
        if(CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, firstAnimationHash))
        {
            // I want to set normalized time to zero since I want the animation to start from the beginning.
            _animator.Play(firstAnimationHash, 0, 0f);
        }
        else
        {
            _animator.CrossFadeInFixedTime(firstAnimationHash, firstCrossFadeTime, 0, 0);
        }
    }

    /// <summary>
    /// Use this instead of calling animator.CrossFadeInFixedTime. This sets certain flags that are needed for the animation to play properly.
    /// </summary>
    public void PlayAnimationWithCrossFadeInFixedTime(int animationHash, float crossFadeDuration = 0.1f)
    {
        waitingForEnd = false;

        _animator.CrossFadeInFixedTime(animationHash, crossFadeDuration);
    }

    public int CurrentAnimHash(int layer)
    {
        AnimatorStateInfo current =
            _animator.GetCurrentAnimatorStateInfo(layer);

        return current.shortNameHash;
    }

    /// <returns>
    /// -1 if not in transition
    /// </returns>
    public int NextAnimationHash(int layer)
    {
        if (_animator.IsInTransition(layer))
        {
            AnimatorStateInfo next =
                _animator.GetNextAnimatorStateInfo(layer);

            return next.shortNameHash;
        }
        return -1;
    }

    public bool IsInTransition(int layer)
    {
        return _animator.IsInTransition(layer);
    }

    // _______________________________________ IsPlaying Checks

    public bool IsPlaying_Idle()
    {
        return CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, _animHash_Idle);
    }

    public bool IsPlaying_Walk()
    {
        return CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, _animHash_Walk);
    }

    public bool IsPlaying_KnockBackBackward()
    {
        return CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, _animHash_KnockBackBackward);
    }

    public void PlayAnimationWithCrossFadeInFixedTimeAndThenPlayAnother(int firstAnimationHash, float firstCrossFadeDuration, int secondAnimationHash, float secondCrossFadeDuration)
    {
        waitingForEnd = false;
        PlayThen(firstAnimationHash, firstCrossFadeDuration, secondAnimationHash, secondCrossFadeDuration);
    }

    // _______________________________________ Play Methods

    public void Play_Idle()
    {
        PlayAnimationWithCrossFadeInFixedTime(_animHash_Idle, 0.1f);
    }

    public void Play_Walk()
    {
        PlayAnimationWithCrossFadeInFixedTime(_animHash_Walk, 0.1f);
    }

    public void Play_KnockBackBackward()
    {
        PlayAnimationWithCrossFadeInFixedTimeAndThenPlayAnother(_animHash_KnockBackBackward, 0.1f, _animHash_Idle, 0.1f);
        
        // TODO: Used for debugging:
        //PlayAnimationWithCrossFadeInFixedTime(_animHash_KnockBackBackward, 1f);
    }

    // _______________________________________ Testing

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


    public void Play_Test1()
    {
        PlayAnimationWithCrossFadeInFixedTime(_animHash_Test1, 1f);
        if (CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, _animHash_Test1))
            Debug.Log("Tried to transition to the same state this was already in.");
    }

    public void Play_Test2()
    {
        PlayAnimationWithCrossFadeInFixedTime(_animHash_Test2, 0.01f);
        if (CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, _animHash_Test2))
            Debug.Log("Tried to transition to the same state this was already in.");
    }

    public void Play_Test3()
    {
        PlayAnimationWithCrossFadeInFixedTime(_animHash_Test3, 1f);
        if (CustomAnimator.IsInOrIsTransitioningToAnimatorState(_animator, 0, _animHash_Test3))
            Debug.Log("Tried to transition to the same state this was already in.");
    }

}
