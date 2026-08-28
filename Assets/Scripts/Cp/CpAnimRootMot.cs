using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpRegisterer cpReg;

    void OnAnimatorMove() {
        CpMgr.inst.data.animDPos[cpReg.Id] = anim.deltaPosition;
        CpMgr.inst.data.animDRot[cpReg.Id] = anim.deltaRotation;
    }
}
