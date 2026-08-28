using Unity.Mathematics;

public static class CpSt_Walk {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.walk,
            0.5f
        );
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        if (CpUtils.SwitchToFallingStIfNotGrounded(id, data))
            return;
        CpUtils.UpdateMovData(
            id,
            data,
            data.input_mov[id],
            float3.zero,
            data.st_Walk_MaxLinSpd[id],
            data.st_Walk_YawSpd[id],
            data.st_Walk_LinAcc[id]
        );
        if (CpInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Dodge,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Dodge);
        else if (CpInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Atk_Light,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CpMgr.inst.ActSt_SwitchState(id, CpUtils.GetLightAtkSt(id, data));
        else if (CpInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Atk_Heavy,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_Jump);
        else if (CpInputBuffer.TryConsumeInput(
            id,
            BufferableInput.Atk_Ult,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime)
        )
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_FlyingAtk);
        else if (math.all(data.input_mov[id] == float2.zero))
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Idle);
    }
}
