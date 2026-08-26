using UnityEngine;

public  class FsmSt_Cc_Dodge : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
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
            CapsuleCharAnimInfo.dodge,
            unityComps[id].animEvents.animEvent,
            0.1f
        );
    }

    public static void Exit(int id, CapsuleChar_BaseData data) {
        data.invul[id] = false;
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        float angSpd = 0;
        if (data.actStSt_ImpactInputRotAllowed[id])
            angSpd = data.st_Dodge_YawSpd[id];
        CapsuleCharActStUtils.UpdateMovData(
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
            if (CapsuleCharActStUtils.SwitchToFallingStIfNotGrounded(id, data))
                return;
            if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Light, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
                CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_HorSlash1);
            else if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Atk_Heavy, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
                CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Atk_Jump);
            else if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Dodge, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime))
                CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Dodge);
        }
    }
}
