using UnityEngine;

/// <summary>
/// Interpolates Transform pos/rot based on rb movement in LateUpdate.
/// </summary>
// TODO: Test this.
public class PhysicsInterpolator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _interpolatePosition = true;
    [SerializeField] private bool _interpolateRotation = true;

    [Header("Refs")]
    [SerializeField] private Rigidbody _sourceRb;
    [SerializeField] private Transform _targetTransform;


    private Vector3 _previousPosition;
    private Vector3 _currentPosition;

    private Quaternion _previousRotation;
    private Quaternion _currentRotation;

    private void Start()
    {
        _previousPosition = _currentPosition = _sourceRb.position;
        _previousRotation = _currentRotation = _sourceRb.rotation;
    }

    private void LateUpdate()
    {
        _previousPosition = _currentPosition;
        _previousRotation = _currentRotation;

        _currentPosition = _sourceRb.position;
        _currentRotation = _sourceRb.rotation;

        // Normalized time since last physics update.
        float t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        t = Mathf.Clamp01(t);

        if (_interpolatePosition)
        {
            _targetTransform.position = Vector3.Lerp(
                _previousPosition,
                _currentPosition,
                t
            );
        }

        if (_interpolateRotation)
        {
            _targetTransform.rotation = Quaternion.Slerp(
                _previousRotation,
                _currentRotation,
                t
            );
        }
    }
}
