using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpHandle cp;

    void OnAnimatorMove() {
        CpMgr.GetAos(cp.Id).animDPos = anim.deltaPosition;
        CpMgr.GetAos(cp.Id).animDRot = anim.deltaRotation;
    }
}
