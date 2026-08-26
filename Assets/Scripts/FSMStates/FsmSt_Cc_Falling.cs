using Unity.Mathematics;
using UnityEngine;

public  class FsmSt_Cc_Falling : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.actStSt_FallingStartHgt[id] = data.trf_pos[id].y;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.falling,
            unityComps[id].animEvents.animEvent,
            4
        );
    }
    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        CapsuleCharActStUtils.UpdateMovData(
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
                CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.FallLanding);
                return;
            }
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
            return;
        }
        if (data.curStDur[id] > 20) {
            // TODO: Character stuck falling. Kill/reset character (maybe have a unique
            // TODO C: death state for when character dies like this where the player doesn't
            // TODO C: lose their souls).
        }
    }
}
