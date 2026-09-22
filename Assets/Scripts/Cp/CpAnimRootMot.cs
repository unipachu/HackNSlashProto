using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpHandle cp;

    void OnAnimatorMove() {
        CpMgr.GetData(cp.I).animDPos = anim.deltaPosition;
        CpMgr.GetData(cp.I).animDRot = anim.deltaRotation;
    }
}
