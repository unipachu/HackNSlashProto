using System;
using Unity.Mathematics;
using UnityEngine;

public  class FsmSt_Cc_Atk_HorSlash2 : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.actStSt_AtkPhase[id] = AtkPhase.Impact;
        data.actStSt_ComboAllowed[id] = false;
        data.actStSt_DodgeAllowed[id] = false;
        data.actStSt_ImpactInputRotAllowed[id] = false;
        data.actStSt_RecoveryMotInterpTimer[id] = 0;
        PcInputBuffer.Clear(id, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.atk_HorSlash2_Impact
        );
    }

    public static void Exit(
        int id,
        CapsuleChar_UnityComps[] unityComps
    ) {
        unityComps[id].rHandEquippable.hitDealer.Deactivate();
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        if (CapsuleCharActStUtils.SwitchToFallingStIfNotGrounded(id, data))
            return;
        switch (data.actStSt_AtkPhase[id]) {
            case AtkPhase.Impact:
                float angSpd = 0;
                if (data.actStSt_ImpactInputRotAllowed[id])
                    angSpd = data.st_AtkHorSlash_Impact_AngSpd[id];
                CapsuleCharActStUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    data.animDPos[id],
                    0,
                    angSpd,
                    float.PositiveInfinity
                );
                if (data.actStSt_ComboAllowed[id]) {
                    if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Light, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime)) {
                        CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_HorSlash3);
                        return;
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                data.actStSt_RecoveryMotInterpTimer[id] += Time.deltaTime;
                // TODO: Make interp value SO field.
                float interpValue = Mathf.Clamp01(data.actStSt_RecoveryMotInterpTimer[id] / 0.2f);
                CapsuleCharActStUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    float3.zero,
                    data.st_Walk_MaxLinSpd[id] * interpValue,
                    data.st_Walk_YawSpd[id] * interpValue,
                    data.st_Walk_LinAcc[id]
                );
                if (data.actStSt_DodgeAllowed[id]) {
                    if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Dodge, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime)) {
                        CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Dodge);
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
