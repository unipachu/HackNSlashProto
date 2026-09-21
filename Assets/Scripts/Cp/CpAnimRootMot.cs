using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpHandle cp;

    void OnAnimatorMove() {
        CpMgr.GetData(cp.Id).animDPos = anim.deltaPosition;
        CpMgr.GetData(cp.Id).animDRot = anim.deltaRotation;
    }
}
