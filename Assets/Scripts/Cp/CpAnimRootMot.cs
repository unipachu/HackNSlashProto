using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    public InterfaceReference<ICp> cp;

    void OnAnimatorMove() {
        cp.Value.CommonData.animDPose.position = anim.deltaPosition;
        cp.Value.CommonData.animDPose.rotation = anim.deltaRotation;
    }
}
