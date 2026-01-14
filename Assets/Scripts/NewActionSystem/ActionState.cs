/// <summary>
/// State used by ActionStateMacine aka ActionController
/// </summary>
public class ActionState
{
    public ActionDefinition CurrentAction;
    public float NormalizedTime;
    /// <summary>
    /// Represents if the action can be cancelled.
    /// </summary>
    public bool IsLocked;

    public void Start(ActionDefinition action)
    {
        CurrentAction = action;
        NormalizedTime = 0f;
        IsLocked = true;
    }

    public void Tick(float normalizedAnimTime)
    {
        NormalizedTime = normalizedAnimTime;

        if (NormalizedTime >= CurrentAction.CanCancelFrom)
            IsLocked = false;
    }

    public bool CanChain(ActionDefinition next)
    {
        if (NormalizedTime < CurrentAction.CanChainFrom)
            return false;

        foreach (var a in CurrentAction.ChainableActions)
            if (a == next) return true;

        return false;
    }

    public bool IsFinished()
    {
        return NormalizedTime >= CurrentAction.EndAt;
    }
}
