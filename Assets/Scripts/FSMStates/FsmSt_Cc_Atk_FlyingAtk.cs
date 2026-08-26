using Unity.Mathematics;
using UnityEngine;

// TODO: This should probably be called FlyingSlam or something more descriptive than "Atk".
public class FsmSt_Cc_Atk_FlyingAtk : MonoBehaviour {
    public static void Enter(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        data.invul[id] = true;
        data.isAffectedByGravity[id] = false;
        data.actStSt_AtkPhase[id] = AtkPhase.Windup;
        data.actStSt_ImpactFinished[id] = false;
        PcInputBuffer.Clear(id, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CapsuleCharAnimInfo.atk_FlyingAtk_Windup,
            unityComps[id].animEvents.animEvent,
            0.1f
        );
    }

    public static void Exit(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps
    ) {
        data.invul[id] = false;
        data.isAffectedByGravity[id] = true;
        unityComps[id].rHandEquippable.aoeHitDealer.Deactivate();
    }

    public static void Tick(
        int id,
        CapsuleChar_BaseData data,
        CapsuleChar_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        switch (data.actStSt_AtkPhase[id]) {
            case AtkPhase.Windup:
                CapsuleCharActStUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    data.animDPos[id],
                    2, // TODO: To So field
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CapsuleCharActStUtils.UpdateMovData(
                    id,
                    data,
                    data.input_mov[id],
                    data.animDPos[id],
                    2, // TODO: To So parameter.
                    0,
                    0
                );
                if (data.isGrounded[id] && data.actStSt_ImpactFinished[id]) {
                    unityComps[id].rHandEquippable.aoeHitDealer.Deactivate();
                    data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref animEventPlrData,
                        unityComps[id].anim,
                        CapsuleCharAnimInfo.atk_FlyingAtk_Recovery,
                        unityComps[id].animEvents.animEvent
                    );
                }
                break;
            case AtkPhase.Recovery:
                CapsuleCharActStUtils.UpdateMovData(
                    id,
                    data,
                    float2.zero,
                    float3.zero,
                    0,
                    0,
                    0
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {data.actStSt_AtkPhase[id]}.");
                break;
        }
    }
}

