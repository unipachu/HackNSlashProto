using System;

/// <summary>
/// Requests the player chaser to stop chasing and start idling.
/// </summary>
[Obsolete]
public class OldBT_Idle : BT_Node
{
    private IPlayerChaser _playerChaser;

    public OldBT_Idle(IPlayerChaser playerChaser)
    {
        _playerChaser = playerChaser;
    }

    public override NodeState Evaluate()
    {
        return _playerChaser.RequestIdle();
    }
}
