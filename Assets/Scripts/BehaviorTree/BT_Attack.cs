using UnityEngine;

public class BT_Attack : BT_Node
{
    private Enemy_Controller _enemy;

    public BT_Attack(Enemy_Controller enemy)
    {
        _enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        return _enemy.RequestAttack();
    }
}
