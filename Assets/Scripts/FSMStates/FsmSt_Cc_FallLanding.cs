using Unity.Mathematics;
using UnityEngine;

public  class FsmSt_Cc_FallLanding : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.actStSt_DodgeAllowed[id] = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.fallLanding,
            // TODO MINOR: You could make this transition faster if falling from higher/faster,
            // TODO MINOR C: e.g. 0.2 if hitting the ground with slow speed, and 0.1 if hitting
            // TODO MINOR C: the ground while fast falling speed.
            0.2f
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
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (data.actStSt_DodgeAllowed[id]) {
            if (PcInputBuffer.TryConsumeInput(id, BufferableInput.Dodge, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime)) {
                CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Dodge);
                return;
            }
        }
    }
}
