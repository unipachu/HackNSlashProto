using Unity.Mathematics;

public static class CpSt_Atk_Jump {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.atk_JumpVerSlam,
            0.1f
        );
    }

    public static void Exit(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        data.isAffectedByGravity[id] = true;
        // TODO: Item
        //unityComps[id].rHandItem.Id.hitDealer.Deactivate();
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
            data.animDPos[id],
            0,
            0,
            float.PositiveInfinity
        );
    }
}
