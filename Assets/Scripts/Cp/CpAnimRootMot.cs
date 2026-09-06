using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpRegisterer cpReg;

    void OnAnimatorMove() {
        CpMgr.inst.soaData.animDPos[cpReg.Id] = anim.deltaPosition;
        CpMgr.inst.soaData.animDRot[cpReg.Id] = anim.deltaRotation;
    }
}
