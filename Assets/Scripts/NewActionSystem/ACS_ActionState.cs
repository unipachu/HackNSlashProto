using UnityEngine;

/// <summary>
/// State used by ActionStateMacine aka ActionController
/// </summary>
public abstract class ACS_ActionState
{
    // TODO: Figure action state data types later.
    //public ActionDefinition CurrentAction;

    //protected abstract ANS_AnimatorState AnimationState { get; }

    /// <summary>
    /// Represents if the action can be cancelled.
    /// </summary>
    public bool IsLocked;

    /// <summary>
    /// Call animation transitions here.
    /// </summary>
    public abstract void EnterState();

    public abstract void UpdateState(float deltaTime);

    //public bool CanChain(ActionDefinition next)
    //{
    //    if (NormalizedTime < CurrentAction.CanChainFrom)
    //        return false;

    //    foreach (var a in CurrentAction.ChainableActions)
    //        if (a == next) return true;

    //    return false;
    //}

    public abstract void ExitState();

    public abstract bool CanInstantlyTransitionTo(ACS_ActionState newAction);
    
    public abstract bool CanBuffer(ACS_ActionState newAction);
    public abstract void BufferAction(ACS_ActionState newAction);
}
