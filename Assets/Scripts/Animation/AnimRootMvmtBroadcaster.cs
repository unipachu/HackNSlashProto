using System;
using UnityEngine;

/// <summary>
/// Used for getting animation root delta movement.
/// </summary>
public class AnimRootMvmtBroadcaster : MonoBehaviour
{
    public event Action<Vector3> OnRootMove;

    [SerializeField] Animator _animator;

    private void OnAnimatorMove()
    {
        Vector3 deltaPos = _animator.deltaPosition;

        OnRootMove?.Invoke(deltaPos);

        // TODO: Using delta position to move root causes small errors (likely floating point errors). There seems
        // TODO CONTD: to be no way to get the exact root position/rotation from the Animator (which is stupid).

        // TODO: Something along the lines of this might work but could be problematic when used with crossfades between animations:

        //AnimationCurve posX;
        //AnimationCurve posY;
        //AnimationCurve posZ;

        //float t = currentAnimationTime;

        //Vector3 rootPos = new Vector3(
        //    posX.Evaluate(t),
        //    posY.Evaluate(t),
        //    posZ.Evaluate(t)
        //);

        //transform.position = startPosition + rootPos;

        // TODO: You should try this but according to the documentation it should not work:         _animator.rootPosition
    }

}
