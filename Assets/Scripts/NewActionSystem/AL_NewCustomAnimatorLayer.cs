using UnityEngine;

public abstract class AL_NewCustomAnimatorLayer
{

    public int LayerIndex { get; }
    public Animator Animator { get; }

    public AN_NewAnimationData InitialState { get; }

    /// <summary>
    /// Currently active state (animation). If in transition, refers to the "next animator state".
    /// NOTE: Animator component refers to its own current/next state -> this offends the SSOT principle.
    /// </summary>
    public AN_NewAnimationData ActiveState { get; private set; }


    public AL_NewCustomAnimatorLayer(int layerIndex, Animator animator, AN_NewAnimationData initialState)
    {
        LayerIndex = layerIndex;
        Animator = animator;
        InitialState = initialState;
    }


    /// <summary>
    /// Uses CrossFadeInFixedTime. <br/>
    /// NOTE: Always call this when you want to transition to new animation. NEVER crossfade or play other animations through any other method.
    /// </summary>
    public void RequestInstantTransitionTo(AN_NewAnimationData nextAnimation, float normalizedTimeOffset = 0, bool startFromBeginning = true)
    {
        //Debug.Log("Transitioning to state: " + nextAnimation.StateName);

        if (IsActiveState(nextAnimation) && !startFromBeginning) return;

        ActiveState = nextAnimation;
        Animator.Play(
            nextAnimation.AnimationHash,
            LayerIndex,
            normalizedTimeOffset
        );
    }

    /// <returns>
    /// NOTE: Checks the current state of the CustomAnimator which might differ from the Animator's state if this code is buggy.
    /// </returns>
    public bool IsActiveState(AN_NewAnimationData state)
    {
        return ActiveState == state;
    }

    /// <summary>
    /// Call this to ask for a new animation.
    /// </summary>
    public void RequestFixedTimeCrossfadeTo(AN_NewAnimationData nextAnimation, float normalizedTimeOffset = 0, bool startFromBeginning = true)
    {
        //Debug.Log("Transitioning to state: " + nextAnimation.StateName);

        if (IsActiveState(nextAnimation) && !startFromBeginning) return;

        ActiveState = nextAnimation;
        Animator.CrossFade(
            nextAnimation.AnimationHash,
            nextAnimation.NormalizedCrossFadeDurationToThis,
            LayerIndex,
            normalizedTimeOffset
        );
    }

    public float GetClampedNormalizedTime()
    {
        return Animator.GetCurrentAnimatorStateInfo(LayerIndex).normalizedTime % 1f;
    }
}


