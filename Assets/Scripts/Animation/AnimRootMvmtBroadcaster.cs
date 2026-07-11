using System;
using UnityEngine;

/// <summary>
/// Used for getting animation root delta movement.
/// </summary>
public class AnimRootMvmtBroadcaster : MonoBehaviour
{
    public event Action<Vector3> OnRootMove;

    [SerializeField] Animator animator;

    private void OnAnimatorMove()
    {
        Vector3 deltaPos = animator.deltaPosition;
        Quaternion deltaRot = animator.deltaRotation;

        OnRootMove?.Invoke(deltaPos);
    }

}
