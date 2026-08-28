using Unity.Mathematics;

public static class CpSt_Idle {
    public static void Enter(int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.idle,
            0.1f
        );
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        if (CpUtils.SwitchToFallingStIfNotGrounded(id, data)) {
            return;
        }
        if (data.prevSt[id] == CpActSt.Walk)
            CpUtils.UpdateMovData(
                id,
                data,
                data.input_mov_LastNonZero[id],
                float3.zero,
                0,
                data.st_Walk_YawSpd[id],
                float.PositiveInfinity
            );
        else
            CpUtils.UpdateMovData(
                id,
                data,
                float2.zero,
                float3.zero,
                0,
                0,
                float.PositiveInfinity
            );
        // Try consume input
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
        else if (CpInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Heavy, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_Jump);
        else if (CpInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Ult, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_FlyingAtk);
        else if (math.all(data.input_mov[id] != float2.zero))
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Walk);
    }
}
