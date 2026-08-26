using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Capsule charater's state machine related utility methods (e.g. state change condition checks).
/// </summary>
public static class CapsuleCharActStUtils{
    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(int id, CapsuleChar_BaseData data) {
        if (!data.isGrounded[id] && data.actSt[id] != CapsuleCharActSt.Falling) {
            CapsuleCharMgr.inst.ActSt_SwitchState(id, CapsuleCharActSt.Falling);
            return true;
        }
        return false;
    }

    public static CapsuleCharActSt GetLightAtkSt(int id, CapsuleChar_BaseData data) {
        switch (data.equip_RHandEquippable[id]) {
            case HandEquippableT.Empty:
                return CapsuleCharActSt.Atk_HorSlash1;
            case HandEquippableT.Sword:
                return CapsuleCharActSt.Atk_HorSlash1;
            case HandEquippableT.Hammer:
                return CapsuleCharActSt.Atk_HorSlash1;
            case HandEquippableT.Pistol:
                return CapsuleCharActSt.Atk_ShootHomingProj;
            default:
                Debug.LogError($"Switch defaulted with {data.equip_RHandEquippable[id]}");
                return CapsuleCharActSt.Atk_HorSlash1;
        }
    }

    public static void UpdateMovData(
        int id,
        CapsuleChar_BaseData data,
        in float2 horMov,
        in float3 animRootMov,
        float maxLinSpd,
        float yawSpd,
        float linAcc
    ) {
        data.mov_horMov[id] = horMov;
        data.mov_animRootMot[id] = animRootMov;
        data.mov_maxLinSpd[id] = maxLinSpd;
        data.mov_yawSpd[id] = yawSpd;
        data.mov_linAcc[id] = linAcc;
    }
}
