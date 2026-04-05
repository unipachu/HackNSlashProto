using System;

/// <summary>
/// Moves towards player.
/// </summary>
[Obsolete]
public class OldBT_ChasePlayer : BT_Node
{
    private IPlayerChaser _playerChaser;

    public OldBT_ChasePlayer(IPlayerChaser playerChaser)
    {
        _playerChaser = playerChaser;
    }

    public override NodeState Evaluate()
    {
        return _playerChaser.RequestChasePlayer();
    }
}
