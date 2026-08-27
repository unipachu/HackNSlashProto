using Unity.Mathematics;
using UnityEngine;

public static class CpSt_Atk_FlyingAtk {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.invul[id] = true;
        data.isAffectedByGravity[id] = false;
        data.actStSt_AtkPhase[id] = AtkPhase.Windup;
        data.actStSt_ImpactFinished[id] = false;
        CpInputBuffer.Clear(id, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.atk_FlyingAtk_Windup,
            0.1f
        );
    }

    public static void Exit(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        data.invul[id] = false;
        data.isAffectedByGravity[id] = true;
        unityComps[id].rHandEquippable.aoeHitDealer.Deactivate();
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        switch (data.actStSt_AtkPhase[id]) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    data.animDPos[id],
                    2, // TODO: To So field
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CpUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    data.animDPos[id],
                    2, // TODO: To So parameter.
                    0,
                    float.PositiveInfinity
                );
                if (data.isGrounded[id] && data.actStSt_ImpactFinished[id]) {
                    unityComps[id].rHandEquippable.aoeHitDealer.Deactivate();
                    data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref animEventPlrData,
                        unityComps[id].anim,
                        CpAnimInfo.atk_FlyingAtk_Recovery
                    );
                }
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovData(
                    id,
                    data,
                    float2.zero,
                    float3.zero,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {data.actStSt_AtkPhase[id]}.");
                break;
        }
    }
}

