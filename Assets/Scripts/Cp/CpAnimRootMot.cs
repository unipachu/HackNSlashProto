using UnityEngine;

public class CpAnimRootMot : MonoBehaviour {
    [SerializeField] Animator anim;
    public InterfaceReference<ICp> cp;

    void OnAnimatorMove() {
        //Dbg.Log(
        //    $"{nameof(anim.deltaPosition)}: {anim.deltaPosition.ToString("G3")}, "
        //        + $"{nameof(anim.deltaRotation)} (euler): {anim.deltaRotation.eulerAngles.ToString("G3")}",
        //    cp.Value.Go,
        //    cp.Value.So_CpCommonConfig.enableDbgMsgs
        //);
        //Dbg.Log(
        //    $"{nameof(anim.deltaPosition)}: was zero!",
        //    cp.Value.Go,
        //    cp.Value.So_CpCommonConfig.enableDbgMsgs && anim.deltaPosition == Vector3.zero
        //);
        //Dbg.Log(
        //    $"{nameof(anim.deltaPosition)}: was not zero!",
        //    cp.Value.Go,
        //    cp.Value.So_CpCommonConfig.enableDbgMsgs && anim.deltaPosition != Vector3.zero
        //);
        //Dbg.Log(
        //    $"applyRootMotion: {anim.applyRootMotion}, "
        //        + $"hasRootMotion: {anim.hasRootMotion}, "
        //        + $"{nameof(anim.deltaPosition)}: {anim.deltaPosition.ToString("G3")}, "
        //        + $"{nameof(anim.deltaRotation)} (euler): {anim.deltaRotation.eulerAngles.ToString("G3")}",
        //    cp.Value.Go,
        //    cp.Value.So_CpCommonConfig.enableDbgMsgs
        //);
        cp.Value.CommonData.animDPose.position = anim.deltaPosition;
        cp.Value.CommonData.animDPose.rotation = anim.deltaRotation;
    }
}
