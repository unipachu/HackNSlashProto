using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpRegisterer cp;

    void OnAnimatorMove() {
        CpMgr.GetSoa(cp.Id).animDPos = anim.deltaPosition;
        CpMgr.GetSoa(cp.Id).animDRot = anim.deltaRotation;
    }
}
