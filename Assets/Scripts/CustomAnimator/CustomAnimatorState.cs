using UnityEngine;

/// <summary>
/// NOTE: For the sake of readibility, add ANIMATION EVENT CALLBACKS (corresponding the state's animation's events)
/// to the animator state classes inheriting from this - but do NOT add logic to them, instead create actions that are invoked.
/// This way if any animation event references break, it's easy to set them up again.
/// NOTE: ANIMATION EVENT CALLBACKS require unique method names among the components in the object that holds the Animator. Therefore
/// prefix or otherwise identify callbacks with the animation name (e.g. AttackAnimation_OnHitActive).
/// </summary>
// TODO MAYBE: Consider EnterState, StartedTransitionToOtherState, and ExitState methods. These could be used to invoke actions,
// TODO CONTD: or limit certain actions from being called during transitions (though this might cause problems if transitioning
// to the same state, since then both the current and next state refer to the same CustomAnimatorState class. Darn.).
public abstract class CustomAnimatorState : MonoBehaviour
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
    /// 
    /// </summary>
    private void Awake()
    {
        OnAwake();
    }

    /// <summary>
    /// This is here just remind you to call InitializeState in the Awake of the inherited class.<br/>
    /// NOTE: This method is run in the base CustomAnimatorState's Awake.
    /// </summary>
    protected abstract void OnAwake();

    /// <summary>
    /// NOTE: Call this in Awake of a implementation of this class.
    /// </summary>
    protected void InitializeState(
        string stateName,
        float crossFadeDurationToThis = 0.1f
        )
    {
        StateName = stateName;
        StateHash = Animator.StringToHash(stateName);
        CrossFadeDurationToThis = crossFadeDurationToThis;
        FallbackState = null;
        FallbackTransitionPrecent = 0.9f;

        Debug.Assert(!string.IsNullOrEmpty(stateName), "State name was null or empty!", this);
    }

    /// <summary>
    /// NOTE: Call this in Awake of a implementation of this class.
    /// </summary>
    protected void InitializeState(
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

        Debug.Assert(FallbackState != null, "Fallback state was null!", this);
        Debug.Assert(GeneralUtils.IsInRange(fallbackTransitionPrecent, 0, 1), "Fallback transition precent was out of range!", this);
        Debug.Assert(!string.IsNullOrEmpty(stateName), "State name was null or empty!", this);
    }
}
