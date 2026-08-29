using Unity.Mathematics;

public static class CpSt_Falling {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.actStSt_FallingStartHgt[id] = data.trf_pos[id].y;
        Dbg.Log("Went here", data.enableDebugMsgs[id]);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.falling,
            4 // TODO: So?
        );
    }
    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        CpUtils.UpdateMovData(
            id,
            data,
            float2.zero,
            float3.zero,
            // TODO MINOR: You could use st_Falling_MaxLinSpd in here + hor input to
            // TODO MINOR: allow for slight air control.
            0,
            0,
            data.st_Falling_LinAcc[id]
        );
        if (data.isGrounded[id]){
            float fallDist = data.actStSt_FallingStartHgt[id] - data.trf_pos[id].y;
            // TODO: Make scriptable object field. This decides if the player will go to
            // TODO C: landing animation or straight to idle.
            if(fallDist > 2) {
                CpMgr.inst.ActSt_SwitchState(id, CpActSt.FallLanding);
                return;
            }
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Idle);
            return;
        }
        if (data.curStDur[id] > 20) {
            // TODO: Character stuck falling. Kill/reset character (maybe have a unique
            // TODO C: death state for when character dies like this where the player doesn't
            // TODO C: lose their souls).
        }
    }
}
