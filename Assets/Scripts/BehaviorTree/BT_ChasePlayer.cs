using UnityEngine;

public class BT_ChasePlayer : BT_Node
{
    private Enemy_Controller _enemyController;

    public BT_ChasePlayer(Enemy_Controller enemyController)
    {
        _enemyController = enemyController;
    }
    public override NodeState Evaluate()
    {

        // TODO: The action state request results dont really mach the node states. Think.
        ActionStateRequestResult result = _enemyController.RequestFullBodyAction(new ACS_Fullbody_ChaseTarget(_enemyController, _enemyController.PlayerTransform, _enemyController.agent));
        switch (result)
        {
            case ActionStateRequestResult.Success:

                return NodeState.Running;
            case ActionStateRequestResult.Failed:
                return NodeState.Failure;
            case ActionStateRequestResult.ActionBuffered:
                // TODO: Does this make sense? Perhaps a new NodeState e.g. NodeState.Queuing would make sense?
                return NodeState.Running;
            default:
                Debug.LogError("Switch case defaulted");
                return NodeState.Failure;
        }
    }
}
