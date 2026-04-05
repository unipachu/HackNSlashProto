using UnityEngine;

public enum ActionStateRequestResult
{
    Success,
    Failed,
    ActionBuffered,
}

/// <summary>
/// State machine for actions.
/// </summary>
// TODO: Make this abstract and create a CharacterActionController, which uses sub action states: body, hands, and fullbody override.
public class ActionController
{
    //public ActionState ActionState = new();
    //public ActionBuffer ActionBuffer = new();

    private ACS_ActionState _previousAction;
    private ACS_ActionState _currentAction;

    //public ACS_ActionState CurrentAction => _currentAction;

    // TODO: Move elsewhere.
    //public bool IsInvulnerable =>
    //    _currentAction != null &&
    //    ActionState.NormalizedTime >= _currentAction.IFrameFrom &&
    //    ActionState.NormalizedTime <= _currentAction.IFrameTo;

    /// <summary>
    /// Updates current action.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateActionController(float deltaTime)
    {
        if (_currentAction != null)
        {
            _currentAction.UpdateState(deltaTime);
            //ActionState.Tick(AnimationDriver.GetActiveStateClampedNormalizedTime());

            //TryConsumeBufferedAction();

        }
        else Debug.LogWarning("Current state was null.");
        // TODO: When attack active, draw debug info.
    }

    /// <summary>
    /// Tries to instantly transition to new state. If instant transition is unsuccessful, tries to buffer action.
    /// </summary>
    public ActionStateRequestResult RequestAction(ACS_ActionState newState)
    {
        //Debug.Log("Requested state : " + newState.GetType().Name);
        //if(_previousAction != null) Debug.Log("Previous state was: " + _previousAction.GetType().Name);
        if (_currentAction == null)
        {
            _currentAction = newState;
            _currentAction.EnterState();
            return ActionStateRequestResult.Success;
        }

        if(_currentAction.CanInstantlyTransitionTo(newState))
        {
            _currentAction.ExitState();
            _previousAction = _currentAction;

            _currentAction = newState;
            _currentAction.EnterState();
            return ActionStateRequestResult.Success;
        }
        else if(_currentAction.CanBuffer(newState))
        {
            _currentAction.BufferAction(newState);
            return ActionStateRequestResult.ActionBuffered;
        }
        return ActionStateRequestResult.Failed;
    }


    // TODO: Move elsewhere.
    //void TryConsumeBufferedAction()
    //{
    //    if (ActionState.IsLocked)
    //        return;

    //    var buffered = ActionBuffer.ConsumeIfValid();
    //    if (buffered.HasValue && ActionState.CanChain(buffered.Value.RequestedAction))
    //    {
    //        StartAction(buffered.Value.RequestedAction);
    //    }
    //}

    //void StartAction(ActionDefinition action)
    //{
    //    _currentAction = action;
    //    ActionState.Start(action);
    //    //AnimationDriver.PlayAction(action);
    //}

    //void EndAction()
    //{
    //    _currentAction = null;
    //    //AnimationDriver.EndAction();
    //}

    //void ForceInterrupt(ActionDefinition action)
    //{
    //    _currentAction = null;
    //    ActionState.IsLocked = false;
    //    ActionBuffer = new ActionBuffer();

    //    StartAction(action);
    //}

    //bool CanInterrupt(ActionDefinition incoming)
    //{
    //    if (_currentAction == null)
    //        return true;

    //    if (_currentAction.Uninterruptible)
    //        return false;

    //    //if (HasHyperArmor())
    //    //    return false;

    //    if ((int)incoming.Priority >= (int)_currentAction.MinPriorityToInterrupt)
    //        return true;

    //    return false;
    //}


    //private string ActionControllerInfo()
    //{
    //    return "Current Action: " + _currentAction.name + "\n"
    //        + "Priority: " + _currentAction.Priority + "\n"
    //        + "Uninterruptible: " + _currentAction.Uninterruptible;
    //}

}
