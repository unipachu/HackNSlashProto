using System.Collections.Generic;

/// <summary>
/// Behaviour tree's selector node. Will go through Evaluate methods of all child nodes until a node return SUCCESS or RUNNING.
/// </summary>
public class BT_Selector : BT_Node
{
    public BT_Selector() : base() { }
    public BT_Selector(List<BT_Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        foreach (BT_Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    continue;
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                default:
                    continue;
            }
        }
        state = NodeState.Failure;
        return state;
    }
}
