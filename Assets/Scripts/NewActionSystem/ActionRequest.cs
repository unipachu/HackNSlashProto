/// <summary>
/// Used to buffer actions.
/// </summary>
public struct ActionRequest
{
    public ActionDefinition RequestedAction;
    /// <summary>
    /// When the action was requested according to Time.time.
    /// </summary>
    public float TimeRequested;
}
