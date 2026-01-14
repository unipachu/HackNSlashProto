using UnityEngine;

// TODO MAYBE: Consider EnterState, StartedTransitionToOtherState, and ExitState methods. These could be used to invoke actions,
// TODO CONTD: or limit certain actions from being called during transitions (though this might cause problems if transitioning
// to the same state, since then both the current and next state refer to the same CustomAnimatorState class. Darn.).

/// <summary>
/// Represents a state of the Animator component.
/// </summary>
public class CustomAnimatorState
{
    /// <summary>
    /// Name of the animation state in the Animator.
    /// </summary>
    public string StateName { get; protected set; }

    /// <summary>
    /// Hash of the animation state name. This should improve performance.
    /// </summary>
    public int StateHash { get; protected set; }

    /// <summary>
    /// Crossfade duration in seconds when transitioning into this state.
    /// </summary>
    public float CrossFadeDurationToThis { get; protected set; }

    /// <summary>
    /// State which this state always transitions to after FallbackTransitionPrecent.<br/>
    /// NOTE: Set this to null if you don't want to automatically exit this state.
    /// </summary>
    // TODO: You could just have an overridable method, that returns null by default, and override this method in inherited classes,
    // TODO CONTD: that also hold a reference to a fallback state.
    public CustomAnimatorState FallbackState { get; protected set; }

    /// <summary>
    /// Percentage (0–1) of the animation at which the fallback transition occurs.
    /// </summary>
    public float FallbackTransitionPrecent { get; protected set; }

    /// <summary>
    /// NOTE: Call this in Awake of a implementation of this class.
    /// </summary>
    public void InitializeState(
        string stateName,
        float crossFadeDurationToThis = 0.1f
        )
    {
        StateName = stateName;
        StateHash = Animator.StringToHash(stateName);
        CrossFadeDurationToThis = crossFadeDurationToThis;
        FallbackState = null;
        FallbackTransitionPrecent = 0.9f;
    }

    /// <summary>
    /// NOTE: Call this in Awake of a implementation of this class.
    /// </summary>
    public void InitializeState(
        string stateName,
        float crossFadeDurationToThis,
        CustomAnimatorState fallbackState,
        float fallbackTransitionPrecent = 0.9f
        )
    {
        StateName = stateName;
        StateHash = Animator.StringToHash(stateName);
        CrossFadeDurationToThis = crossFadeDurationToThis;
        FallbackState = fallbackState;
        FallbackTransitionPrecent = fallbackTransitionPrecent;
    }
}
