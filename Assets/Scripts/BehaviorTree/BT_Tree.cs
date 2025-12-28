using UnityEngine;

/// <summary>
/// Base class for behavior trees.
/// </summary>
public abstract class BT_Tree : MonoBehaviour
{
    private BT_Node _root = null;

    protected virtual void Start()
    {
        _root = SetupTree();
    }

    protected virtual void Update()
    {
        if (_root != null)
            _root.Evaluate();
    }

    /// <summary>
    /// Create the (initial) nodes of the tree here.
    /// </summary>
    /// <returns>
    /// Root node of the tree.
    /// </returns>
    protected abstract BT_Node SetupTree();
}
