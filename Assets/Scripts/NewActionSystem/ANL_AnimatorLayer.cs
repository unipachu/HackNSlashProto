using System;
using Unity.AppUI.UI;
using UnityEngine;

/// <summary>
/// NOTE: Inherited classes should include all animator states as public read-only auto-implemented properties.
/// </summary>
public abstract class ANL_AnimatorLayer
{
    /// <summary>
    /// Last normalized time this state was updated. Set to -1 when state change is called to indicate that the state
    /// has not been updated yet.
    /// </summary>
    protected float _previousNormalizedTime = -1;

    public int LayerIndex { get; }
    public Animator Animator { get; }
    public ANS_AnimatorState InitialState { get; }
    /// <summary>
    /// Currently active state (animation). If in transition, refers to the "next animator state".
    /// NOTE: Animator component refers to its own current/next state -> this offends the SSOT principle.
    /// </summary>
    public ANS_AnimatorState ActiveState { get; private set; }

    /// <summary>
    /// Use this to initialize <see cref="ANS_AnimatorState"/> instances in child classes.<br/>
    /// NOTE: You need to initialize the 
    /// </summary>
    /// <param name="initialState">NOTE: After initializing the initial state as a parameter, you should save it
    /// to a field.
    /// </param>
    public ANL_AnimatorLayer(int layerIndex, Animator animator, ANS_AnimatorState initialState)
    {
        if (animator == null)
            throw new ArgumentNullException(nameof(animator), "Animator cannot be null.");

        if (initialState == null)
            throw new ArgumentNullException(nameof(initialState), "Initial state cannot be null.");

        if (layerIndex < 0 || layerIndex >= animator.layerCount)
            throw new ArgumentOutOfRangeException(nameof(layerIndex),
                $"Layer index {layerIndex} is out of range. Animator has {animator.layerCount} layers.");

        LayerIndex = layerIndex;
        Animator = animator;
        InitialState = initialState;
    }

    /// <summary>
    /// Use this in inherited classes to invoke animation event and animation range events.<br/>
    /// NOTE: Call base method at the end of an override to update <see cref="_previousNormalizedTime"/>.
    /// </summary>
    public virtual void UpdateLayer()
    {
        // Do everything before setting previous normalized time!
        _previousNormalizedTime = GetActiveAnimatorStateInfo().normalizedTime;
    }

    /// <summary>
    /// Uses <see cref="Animator.Play(int, int, float)"/> to transition to new Animator state. <br/>
    /// NOTE: Always call this when you want to transition to new animation. NEVER crossfade or play other animations through any other method.
    /// </summary>
    public void RequestInstantTransitionTo(ANS_AnimatorState nextAnimation, float normalizedTimeOffset = 0)
    {
        //Debug.Log("Transitioning to state: " + nextAnimation.AnimationName);

        // Set this to -1 to signal that the new state hasn't been updated once.
        // TODO: This AND OTHER TRANSITION METHODS set this to -1. Either set it to normalizedTimeOffset and save elsewhere
        // TODO C: flag is this is the first state update, or save "normalizedStartOffset" somewhere.
        _previousNormalizedTime = -1;

        ActiveState = nextAnimation;
        Animator.Play(
            nextAnimation.AnimationHash,
            LayerIndex,
            normalizedTimeOffset
        );
    }

    /// <summary>
    /// Uses <see cref="Animator.CrossFade(int, float, int, float)"/> to crossfade to new animation in normalized time.
    /// </summary>
    public void RequestCrossfadeTo(ANS_AnimatorState nextAnimation, float normalizedTimeOffset = 0)
    {
        //Debug.Log("Transitioning to state: " + nextAnimation.AnimationName);

        // Set this to -1 to signal that the new state hasn't been updated once.
        _previousNormalizedTime = -1;

        ActiveState = nextAnimation;
        Animator.CrossFade(
            nextAnimation.AnimationHash,
            nextAnimation.NormalizedCrossFadeDurationToThis,
            LayerIndex,
            normalizedTimeOffset
        );
    }

    /// <summary>
    /// Checks the current state of the CustomAnimator which might differ from the Animator's state if this code is buggy.
    /// </summary>
    public bool IsActiveState(ANS_AnimatorState state)
    {
        return ActiveState == state;
    }

    /// <summary>
    /// Normalized time of the active animation (current/next animation depending of if the Animator is in Transition).<br/>
    /// NOTE: Normalized time is not clamped and can go over 1.
    /// </summary>
    public float GetActiveStateNormalizedTime()
    {
        return GetActiveAnimatorStateInfo().normalizedTime;
    }

    /// <summary>
    /// Normalized time of the active animator state (current/next state depending of if the Animator is in transition)
    /// clamped to 0-1 (inclusive).
    /// </summary>
    public float GetActiveStateClampedNormalizedTime()
    {
        return GetActiveAnimatorStateInfo().normalizedTime % 1f;
    }

    /// <summary>
    /// Previous normalized time of the active animator state
    /// clamped to 0-1 (inclusive) or -1 if active state hasn't been updated once.
    /// </summary>
    public float GetPreviousClampedNormalizedTime()
    {
        if (_previousNormalizedTime < 0)
            return -1;
        return _previousNormalizedTime % 1f;
    }

    /// <returns>
    /// Time in seconds until the end of the animation (when clamped normalized time == 1). Used e.g. for buffering combo attacks during attack animations.
    /// </returns>
    public float GetFixedTimeUntilAnimationEnd()
    {
        return (1f - GetActiveStateClampedNormalizedTime()) * ActiveState.GetFixedDuration();
    }

    /// <summary>
    /// Gets the current animator state info if not in transition, and the next animator state info if in transition.
    /// </summary>
    public AnimatorStateInfo GetActiveAnimatorStateInfo()
    {
        if(Animator.IsInTransition(LayerIndex))
            return Animator.GetNextAnimatorStateInfo(LayerIndex);
        return Animator.GetCurrentAnimatorStateInfo(LayerIndex);
    }
}


