using UnityEngine;

/// <summary>
/// Checks if the this object is in the specified range to some other object.
/// </summary>
// TODO: Rename class to something like "FindATarget". Then create separate nodes for checking if in range for chasing.
public class BT_IsInRange : BT_Node
{
    private float _minDistance;
    private float _maxDistance;
    private Transform _thisTransform;
    private Transform _otherTransform;

    public BT_IsInRange(float minDistance, float maxDistance, Transform thisTransform, Transform otherTransform)
    {
        _minDistance = minDistance;
        _maxDistance = maxDistance;
        _thisTransform = thisTransform;
        _otherTransform = otherTransform;
    }

    public override NodeState Evaluate()
    {
        if(_otherTransform == null)
        {
            Debug.LogWarning("Other transform was null. This transform was: " + _thisTransform.name);
            return NodeState.Failure;
        }

        if(GeneralUtils.IsWithinAllowedDist(_thisTransform.position, _otherTransform.position, _minDistance, _maxDistance))
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
