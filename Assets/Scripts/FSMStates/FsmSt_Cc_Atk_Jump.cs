using Unity.Mathematics;
using UnityEngine;

public  class FsmSt_Cc_Atk_Jump : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.atk_JumpVerSlam,
            0.1f
        );
    }

    public static void Exit(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        data.isAffectedByGravity[id] = true;
        unityComps[id].rHandEquippable.hitDealer.Deactivate();
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        CapsuleCharActStUtils.UpdateMovData(
            id,
            data,
            float2.zero,
            data.animDPos[id],
            0,
            0,
            float.PositiveInfinity
        );
    }
}
