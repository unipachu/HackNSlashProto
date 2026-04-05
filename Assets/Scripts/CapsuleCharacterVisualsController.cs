using System;
using UnityEngine;

/// <summary>
/// Used for getting animation root delta movement.
/// </summary>
public class CapsuleCharacterVisualsController : MonoBehaviour
{
    public event Action<Vector3> OnRootMove;

    [SerializeField] Animator _animator;

    private void OnAnimatorMove()
    {
        Vector3 deltaPos = _animator.deltaPosition;

        OnRootMove?.Invoke(deltaPos);
    }
}
