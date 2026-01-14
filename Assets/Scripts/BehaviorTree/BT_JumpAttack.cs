public class BT_JumpAttack : BT_Node
{
    private IJumpAttacker _jumpAttacker;

    public BT_JumpAttack(IJumpAttacker jumpAttacker)
    {
        _jumpAttacker = jumpAttacker;
    }

    public override NodeState Evaluate()
    {
        return _jumpAttacker.RequestJumpAttack();
    }
}
