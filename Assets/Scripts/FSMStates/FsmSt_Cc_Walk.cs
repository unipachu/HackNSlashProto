using Unity.Mathematics;
using UnityEngine;

public  class FsmSt_Cc_Walk : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.walk,
            0.5f
        );
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        if (CapsuleCharActStUtils.SwitchToFallingStIfNotGrounded(id, data))
            return;
        CapsuleCharActStUtils.UpdateMovData(
            id,
            data,
            data.input_mov[id],
            float3.zero,
            data.st_Walk_MaxLinSpd[id],
            data.st_Walk_YawSpd[id],
            data.st_Walk_LinAcc[id]
        );
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
        else if (PcInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Atk_Heavy,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_Jump);
        else if (PcInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Atk_Ult,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_FlyingAtk);
        else if (math.all(data.input_mov[id] == float2.zero))
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
    }
}
