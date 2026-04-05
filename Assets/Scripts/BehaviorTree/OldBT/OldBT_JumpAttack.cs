using System;

[Obsolete]
public class OldBT_JumpAttack : BT_Node
{
    private IJumpAttacker _jumpAttacker;

    public OldBT_JumpAttack(IJumpAttacker jumpAttacker)
    {
        _jumpAttacker = jumpAttacker;
    }

    public override NodeState Evaluate()
    {
        return _jumpAttacker.RequestJumpAttack();
    }
}
