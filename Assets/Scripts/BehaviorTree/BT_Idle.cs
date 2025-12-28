/// <summary>
/// Requests the player chaser to stop chasing and start idling.
/// </summary>
public class BT_Idle : BT_Node
{
    private IPlayerChaser _playerChaser;

    public BT_Idle(IPlayerChaser playerChaser)
    {
        _playerChaser = playerChaser;
    }

    public override NodeState Evaluate()
    {
        return _playerChaser.RequestIdle();
    }
}
