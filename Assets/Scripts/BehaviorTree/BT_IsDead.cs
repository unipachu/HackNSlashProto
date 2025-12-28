public class BT_IsDead : BT_Node
{
    private IHittable _hittable;

    public BT_IsDead(IHittable hittable)
    {
        _hittable = hittable;
    }

    public override NodeState Evaluate()
    {
        if (_hittable.IsDead)
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
