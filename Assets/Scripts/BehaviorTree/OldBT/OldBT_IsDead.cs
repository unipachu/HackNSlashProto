using System;

[Obsolete]
public class OldBT_IsDead : BT_Node
{
    private IHittable _hittable;

    public OldBT_IsDead(IHittable hittable)
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
