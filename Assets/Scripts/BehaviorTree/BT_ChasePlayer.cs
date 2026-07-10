using UnityEngine;

public class BT_ChasePlayer : BT_Node
{
    private Enemy_Brain enemyBrain;

    public BT_ChasePlayer(PC enemyController)
    {
        //enemyBrain = enemyController;
    }
    public override NodeState Evaluate()
    {

        //// TODO: The action state request results dont really mach the node states. Think.
        //ActionStateRequestResult result = enemyBrain.RequestFullBodyAction(
        //    new ACS_Fullbody_ChaseTarget(enemyBrain, enemyBrain.PlayerTransform, enemyBrain.agent));
        //switch (result)
        //{
        //    case ActionStateRequestResult.Success:

        //        return NodeState.Running;
        //    case ActionStateRequestResult.Failed:
        //        return NodeState.Failure;
        //    case ActionStateRequestResult.ActionBuffered:
        //        // TODO: Does this make sense? Perhaps a new NodeState e.g. NodeState.Queuing would make sense?
        //        return NodeState.Running;
        //    default:
        //        Debug.LogError("Switch case defaulted");
        //        return NodeState.Failure;
        //}
        Debug.LogError("Not implemented");
        return NodeState.Failure;
    }
}
