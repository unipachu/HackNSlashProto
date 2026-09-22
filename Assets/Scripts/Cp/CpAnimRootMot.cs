using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CpHandle cp;

    void OnAnimatorMove() {
        cp.Data.animDPos = anim.deltaPosition;
        cp.Data.animDRot = anim.deltaRotation;
    }
}
