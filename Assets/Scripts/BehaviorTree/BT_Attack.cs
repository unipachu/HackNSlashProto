using UnityEngine;

public class BT_Attack : BT_Node
{
    private Enemy_Pawn _enemy;

    public BT_Attack(Enemy_Pawn enemy)
    {
        _enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        return _enemy.RequestAttack();
    }
}
