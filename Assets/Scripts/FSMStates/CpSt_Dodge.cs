using UnityEngine;

public static class CpSt_Dodge {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        // TODO MINOR: Rename to yawinputrotallowed
        data.actStSt_ImpactInputRotAllowed[id] = false;
        data.actStSt_BufferedInputStSwitchAllowed[id] = false;
        data.invul[id] = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            // TODO MINOR: Rename from dodge to Dodge
            CpAnimInfo.dodge,
            0.1f
        );
    }

    public static void Exit(int id, Cp_BaseData data) {
        data.invul[id] = false;
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        float angSpd = 0;
        if (data.actStSt_ImpactInputRotAllowed[id])
            angSpd = data.st_Dodge_YawSpd[id];
        CpUtils.UpdateMovData(
            id,
            data,
            data.input_mov[id],
            data.animDPos[id],
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (data.actStSt_BufferedInputStSwitchAllowed[id]){
            // NOTE: not buffered input but whatever. TODO: REfactor
            if (CpUtils.SwitchToFallingStIfNotGrounded(id, data))
                return;
            if (CpInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Light, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
                CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_HorSlash1);
            else if (CpInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Heavy, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
                CpMgr.inst.ActSt_SwitchState(id, CpActSt.Atk_Jump);
            else if (CpInputBuffer.TryConsumeInput(id, BufferableInput.Dodge, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
                CpMgr.inst.ActSt_SwitchState(id, CpActSt.Dodge);
        }
    }
}
