using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Capsule pawn general util methods. Consider organizing these better!
/// </summary>
public static class CpUtils{
    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(int id, Cp_BaseData data) {
        if (!data.isGrounded[id] && data.actSt[id] != CpActSt.Falling) {
            //Debug.Log($"{id} was not grounded so switch to falling st!");
            CpMgr.inst.ActSt_SwitchState(id, CpActSt.Falling);
            return true;
        }
        return false;
    }

    public static CpActSt GetLightAtkSt(int id, Cp_BaseData data) {
        // TODO: Item

        //switch (data.equip_RHandEquippable[id]) {
        //    case HandItemT.Empty:
        //        return CpActSt.Atk_HorSlash1;
        //    case HandItemT.Sword:
        //        return CpActSt.Atk_HorSlash1;
        //    case HandItemT.Hammer:
        //        return CpActSt.Atk_HorSlash1;
        //    case HandItemT.Gun:
        //        return CpActSt.Atk_ShootHomingProj;
        //    default:
        //        Debug.LogError($"Switch defaulted with {data.equip_RHandEquippable[id]}");
        //        return CpActSt.Atk_HorSlash1;
        //}
        return CpActSt.Atk_HorSlash1;
    }

    public static void UpdateMovData(
        int id,
        Cp_BaseData data,
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
