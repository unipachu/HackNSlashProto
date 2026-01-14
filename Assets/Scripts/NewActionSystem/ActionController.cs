using UnityEngine;

/// <summary>
/// State machine for actions.
/// </summary>
// TODO: Make this abstract and create a CharacterActionController, which uses sub action states: body, hands, and fullbody override.
public class ActionController : MonoBehaviour
{
    public ActionState ActionState = new();
    public ActionBuffer ActionBuffer = new();
    public NewAnimationDriver AnimationDriver;

    private ActionDefinition _currentAction;

    public ActionDefinition CurrentAction => _currentAction;

    public float NormalizedTime => ActionState.NormalizedTime;

    public bool IsInvulnerable =>
        _currentAction != null &&
        ActionState.NormalizedTime >= _currentAction.IFrameFrom &&
        ActionState.NormalizedTime <= _currentAction.IFrameTo;

    void Update()
    {
        if (_currentAction != null)
        {
            ActionState.Tick(AnimationDriver.GetNormalizedTime());

            TryConsumeBufferedAction();

            if (ActionState.IsFinished())
                EndAction();
        }
        // TODO: When attack active, draw debug info.
    }

    public void RequestAction(ActionDefinition action)
    {
        Debug.Log("Requested action: " + action.name, this);

        if (_currentAction == null)
        {
            StartAction(action);
            return;
        }

        if (CanInterrupt(action))
        {
            ForceInterrupt(action);
            return;
        }

        if (CanStartAction(action))
        {
            StartAction(action);
        }
        else
        {
            ActionBuffer.Buffer(action);
        }
    }

    void TryConsumeBufferedAction()
    {
        if (ActionState.IsLocked)
            return;

        var buffered = ActionBuffer.ConsumeIfValid();
        if (buffered.HasValue && ActionState.CanChain(buffered.Value.RequestedAction))
        {
            StartAction(buffered.Value.RequestedAction);
        }
    }

    bool CanStartAction(ActionDefinition action)
    {
        if (_currentAction == null)
            return true;

        return !ActionState.IsLocked && ActionState.CanChain(action);
    }

    void StartAction(ActionDefinition action)
    {
        _currentAction = action;
        ActionState.Start(action);
        AnimationDriver.PlayAction(action);
    }

    void EndAction()
    {
        _currentAction = null;
        AnimationDriver.EndAction();
    }

    void ForceInterrupt(ActionDefinition action)
    {
        _currentAction = null;
        ActionState.IsLocked = false;
        ActionBuffer = new ActionBuffer();

        StartAction(action);
    }

    bool CanInterrupt(ActionDefinition incoming)
    {
        if (_currentAction == null)
            return true;

        if (_currentAction.Uninterruptible)
            return false;

        if (HasHyperArmor())
            return false;

        if ((int)incoming.Priority >= (int)_currentAction.MinPriorityToInterrupt)
            return true;

        return false;
    }

    public bool HasHyperArmor()
    {
        float t = ActionState.NormalizedTime;
        return t >= _currentAction.HyperArmorFrom &&
               t <= _currentAction.HyperArmorTo;
    }

    private string ActionControllerInfo()
    {
        return "Current Action: " + _currentAction.name + "\n"
            + "Priority: " + _currentAction.Priority + "\n"
            + "Uninterruptible: " + _currentAction.Uninterruptible + "\n"
            + "Hyper Armor: " + HasHyperArmor();
    }

}
