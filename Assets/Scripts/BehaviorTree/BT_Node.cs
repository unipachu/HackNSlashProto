using System.Collections.Generic;

public enum NodeState
{
    Running,
    Success,
    Failure
}

/// <summary>
/// Abstract class representing a behavior tree node used by enemies.
/// </summary>
public abstract class BT_Node
{
    public BT_Node parent;

    // TODO: Possibly remove this:
    protected NodeState state;
    protected List<BT_Node> children = new List<BT_Node>();

    public BT_Node()
    {
        parent = null;
    }

    public BT_Node(List<BT_Node> children)
    {
        foreach (BT_Node child in children)
            Attach(child);
    }

    /// <summary>
    /// Attach a child node to this node.
    /// </summary>
    /// <param name="node">
    /// Node to be added as this node's child.
    /// </param>
    private void Attach(BT_Node node)
    {
        node.parent = this;
        children.Add(node);
    }

    /// <summary>
    /// Main method where the node determines which tasks to perform and executes them.
    /// </summary>
    /// <returns>
    /// Returns "failure" by default but is meant to be overridden.
    /// </returns>
    public virtual NodeState Evaluate() => NodeState.Failure;
}
