using UnityEngine;

public  class FsmSt_Cc_Atk_ShootHomingProj : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.atk_GunShoot_Windup,
            0.1f
        );
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        // TODO: Add to So
        CapsuleCharActStUtils.UpdateMovData(
            id,
            data,
            data.input_mov_LastNonZero[id],
            data.animDPos[id],
            0,
            180,
            float.PositiveInfinity
        );
    }
}
