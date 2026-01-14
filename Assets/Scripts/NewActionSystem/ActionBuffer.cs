using UnityEngine;

/// <summary>
/// Can be used to buffer one action.
/// </summary>
public class ActionBuffer
{
    ActionRequest? _bufferedAction;
    /// <summary>
    /// Duration the buffered action can be consumed.
    /// </summary>
    // TODO: Maybe later you could have the current action decide if action can be buffered or not?
    float _bufferDuration = 0.25f;

    public void Buffer(ActionDefinition action)
    {
        _bufferedAction = new ActionRequest
        {
            RequestedAction = action,
            TimeRequested = Time.time
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// Buffered action if such exists, otherwise null.
    /// </returns>
    public ActionRequest? ConsumeIfValid()
    {
        if (!_bufferedAction.HasValue)
            return null;

        if (Time.time - _bufferedAction.Value.TimeRequested > _bufferDuration)
        {
            _bufferedAction = null;
            return null;
        }

        var req = _bufferedAction;
        _bufferedAction = null;
        return req;
    }

    public bool HasBufferedAction => _bufferedAction.HasValue;
}
