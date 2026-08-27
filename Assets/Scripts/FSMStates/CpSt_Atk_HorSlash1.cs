using Unity.Mathematics;
using UnityEngine;

public static class CpSt_Atk_HorSlash1 {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.actStSt_AtkPhase[id] = AtkPhase.Windup;
        data.actStSt_ComboAllowed[id] = false;
        data.actStSt_DodgeAllowed[id] = false;
        data.actStSt_ImpactInputRotAllowed[id] = false;
        data.actStSt_RecoveryMotInterpTimer[id] = 0;
        CpInputBuffer.Clear(id, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.atk_HorSlash1_Windup,
            0.1f
        );
    }

    public static void Exit(
        int id,
        Cp_UnityComps[] unityComps
    ) {
        unityComps[id].rHandEquippable.hitDealer.Deactivate();
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        if (CpUtils.SwitchToFallingStIfNotGrounded(id, data))
            return;
        switch (data.actStSt_AtkPhase[id]) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov_WhenLastSwitchedSt[id],
                    data.animDPos[id],
                    0,
                    data.st_AtkHorSlash_Windup_MaxAngSpd[id],
                    float.PositiveInfinity
                );
                return;
            case AtkPhase.Impact:
                float angSpd = 0;
                if (data.actStSt_ImpactInputRotAllowed[id])
                    angSpd = data.st_AtkHorSlash_Impact_AngSpd[id];
                CpUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    data.animDPos[id],
                    0,
                    angSpd,
                    float.PositiveInfinity
                );
                if (data.actStSt_ComboAllowed[id]) {
                    if(CpInputBuffer.TryConsumeInput(
                        id,
                        BufferableInput.Atk_Light,
                        data.inputBuffer_BufferedInput,
                        data.inputBuffer_RemainingTime)
                    ) {
                        CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_HorSlash2);
                        return;
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                data.actStSt_RecoveryMotInterpTimer[id] += Time.deltaTime;
                // TODO: Make interp value SO field.
                float interpValue = Mathf.Clamp01(data.actStSt_RecoveryMotInterpTimer[id] / 0.2f);
                CpUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    float3.zero,
                    data.st_Walk_MaxLinSpd[id] * interpValue,
                    data.st_Walk_YawSpd[id] * interpValue,
                    data.st_Walk_LinAcc[id]
                );
                if (data.actStSt_DodgeAllowed[id]) {
                    if (CpInputBuffer.TryConsumeInput(
                        id,
                        BufferableInput.Dodge,
                        data.inputBuffer_BufferedInput,
                        data.inputBuffer_RemainingTime)
                    ) {
                        CpMgr.inst.ActSt_SwitchState(id, CpActSt.Dodge);
                        return;
                    }
                }
                return;
            default:
                Debug.LogError($"Switch defaulted with {data.actStSt_AtkPhase[id]}.");
                return;
        }
    }
}
