using Unity.Mathematics;
using UnityEngine;

public  class FsmSt_Cc_Idle : MonoBehaviour {
    public static void Enter(int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.idle,
            unityComps[id].animEvents.animEvent,
            0.1f
        );
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        if (CapsuleCharActStUtils.SwitchToFallingStIfNotGrounded(id, data))
            return;
        if (data.prevSt[id] == CapsuleCharActSt.Walk)
            CapsuleCharActStUtils.UpdateMovData(
                id,
                data,
                data.input_mov_LastNonZero[id],
                float3.zero,
                0,
                data.st_Walk_YawSpd[id],
                float.PositiveInfinity
            );
        else
            CapsuleCharActStUtils.UpdateMovData(
                id,
                data,
                float2.zero,
                float3.zero,
                0,
                0,
                0
            );
        // Try consume input
        if (PcInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Dodge,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Dodge);
        else if (PcInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Atk_Light,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActStUtils.GetLightAtkSt(id, data));
        else if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Heavy, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_Jump);
        else if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Ult, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_FlyingAtk);
        else if (!data.input_mov.Equals(float2.zero))
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
    }
}
