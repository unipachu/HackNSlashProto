/// <summary>
/// Moves towards player.
/// </summary>
public class BT_ChasePlayer : BT_Node
{
    private IPlayerChaser _playerChaser;

    public BT_ChasePlayer(IPlayerChaser playerChaser)
    {
        _playerChaser = playerChaser;
    }

    public override NodeState Evaluate()
    {
        return _playerChaser.RequestChasePlayer();
    }
}
