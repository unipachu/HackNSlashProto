using System.Collections.Generic;

/// <summary>
/// Behaviour tree's sequence node. Will go through Evaluate methods of all child nodes until a node returns FAILURE. If no FAILURES then returns RUNNING if any children were RUNNING. If all children returned SUCCESS, returns SUCCESS.
/// </summary>
public class BT_Sequence : BT_Node
{
    public BT_Sequence() : base() { }
    public BT_Sequence(List<BT_Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;

        foreach (BT_Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildIsRunning = true;
                    continue;
                default:
                    state = NodeState.Success;
                    return state;
            }
        }
        state = anyChildIsRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}
