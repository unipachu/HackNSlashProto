using System;
using UnityEngine;

/// <summary>
/// Used for getting animation root delta movement.
/// </summary>
public class AnimRootMovBroadcaster : MonoBehaviour{
    public event Action<Vector3, Quaternion> OnRootMove;

    [SerializeField] Animator anim;

    void OnAnimatorMove(){
        Vector3 dPos = anim.deltaPosition;
        Quaternion dRot = anim.deltaRotation;
        OnRootMove?.Invoke(dPos, dRot);
    }

}
