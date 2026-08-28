public  static class CpSt_Atk_ShootHomingProj {
    public static void Enter(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps,
        ref AnimEventPlrData animEventPlrData
    ) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref animEventPlrData,
            unityComps[id].anim,
            CpAnimInfo.atk_GunShoot_Windup,
            0.1f
        );
    }

    public static void Tick(
        int id,
        Cp_BaseData data,
        Cp_UnityComps[] unityComps
    ) {
        // TODO: Add to So
        CpUtils.UpdateMovData(
            id,
            data,
            data.input_mov_LastNonZero[id],
            data.animDPos[id],
            0,
            180,
            float.PositiveInfinity
        );
    }
}
