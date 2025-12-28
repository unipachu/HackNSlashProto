using UnityEngine;

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

        float distance = Vector3.Distance(_thisTransform.position, _otherTransform.position);
        if (distance <= _maxDistance && distance >= _minDistance)
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
