using Unity.Mathematics;

public static class CpSt_FallLanding {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.actStSt_DodgeAllowed[id] = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.fallLanding,
            // TODO MINOR: You could make this transition faster if falling from higher/faster,
            // TODO MINOR C: e.g. 0.2 if hitting the ground with slow speed, and 0.1 if hitting
            // TODO MINOR C: the ground while fast falling speed.
            0.2f
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
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (data.actStSt_DodgeAllowed[id]) {
            if (CpInputBuffer.TryConsumeInput(id, BufferableInput.Dodge, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime)) {
                CpMgr.inst.ActSt_SwitchState(id, CpActSt.Dodge);
                return;
            }
        }
    }
}
