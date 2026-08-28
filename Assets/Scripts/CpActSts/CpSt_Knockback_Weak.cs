using Unity.Mathematics;
using UnityEngine;

public static class CpSt_Knockback_Weak {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        Vector3 viewVec = new Vector3(
            data.lastRecievedHitDir[id].x,
            0,
            data.lastRecievedHitDir[id].z
        );
        // If you, for some reason, set the hit direction to Vector3.zero.
        if (viewVec.sqrMagnitude < 0.0001f)
            viewVec = Vector3.down;
        else
            viewVec.Normalize();
        if (Vector3.Dot(data.lastRecievedHitDir[id], unityComps[id].rootTrf.forward) > 0) {
            AnimEventPlr.CrossfadeNInitAnimEventPlr(
                ref animEventPlrData,
                unityComps[id].anim,
                CpAnimInfo.knockback_Weak_Fwd,
                0.1f
            );
        }
        else {
            AnimEventPlr.CrossfadeNInitAnimEventPlr(
                ref animEventPlrData,
                unityComps[id].anim,
                CpAnimInfo.knockback_Weak_Bwd,
                0.1f
            );
        }
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        //if (CapsuleCharActStUtils.SwitchToFallingStIfNotGrounded(id, data))
        //    return;
        CpUtils.UpdateMovData(
            id,
            data,
            float2.zero,
            data.animDPos[id] * data.lastKnockbackStr[id],
            0,
            0,
            float.PositiveInfinity
        );
    }
}
