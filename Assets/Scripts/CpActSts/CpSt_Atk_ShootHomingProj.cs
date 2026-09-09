using UnityEngine;

public class CpSt_Atk_ShootHomingProj : IFsmSt_Cp{
    int cpId;

    public CpSt_Atk_ShootHomingProj(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
    => true;

    // TODO: Use IGun info to get info about what kind of projectile will be spawned and to where. Then
    // TODO C: handle it in an animation event.
    public CpSt_Atk_ShootHomingProj Enter() {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.atk_GunShoot_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var data = CpMgr.inst.soaData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (data.actStSt_AtkPhase[cpId]) {
                    case AtkPhase.Windup:
                        // TODO: Make into So data
                        HomingProjMovData projData;
                        HitEffects atkData = new(10, KnockbackT.Weak, 0.5f);
                        float spd = 5;
                        float maxLifetime = 10;
                        float homingStr = 2;
                        projData = new(spd, maxLifetime, homingStr);
                        // TODO: Item
                        //HomingProjMgr.inst.ShootProj(
                        //    projData,
                        //    atkData,
                        //    unityComps.rHandItem.projSpawnPose.position,
                        //    unityComps.rHandItem.projSpawnPose.forward,
                        //    // TODO: Set homing target.
                        //    null
                        //);
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cpId],
                            CpMgr.inst.unityComps[cpId].anim,
                            CpAnimInfo.Get(CpAnimInfoT.atk_GunShoot_Recovery)
                        );
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cpId);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {data.actStSt_AtkPhase[cpId]}");
                        break;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        CpUtils.UpdateMovData(
            cpId,
            data,
            data.input_mov_LastNonZero[cpId],
            data.animDPos[cpId],
            0,
            // TODO: Add to So
            180,
            float.PositiveInfinity
        );
    }
}
