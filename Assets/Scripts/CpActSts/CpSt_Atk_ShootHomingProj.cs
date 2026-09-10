using UnityEngine;

public class CpSt_Atk_ShootHomingProj : IFsmSt_Cp{
    int cpId;
    HitEffects hitEffects;
    HomingProjData homingProjData;
    Transform projSpawnPose;
    Transform homingProjTgt;

    public CpSt_Atk_ShootHomingProj(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    // TODO: Use IGun info to get info about what kind of projectile will be spawned and to where. Then
    // TODO C: handle it in an animation event.
    public CpSt_Atk_ShootHomingProj Enter(
        HitEffects hitEffects,
        HomingProjData homingProjData,
        Transform projSpawnPose,
        Transform homingProjTgt
    ) {
        this.hitEffects = hitEffects;
        this.homingProjData = homingProjData;
        this.projSpawnPose = projSpawnPose;
        this.homingProjTgt = homingProjTgt;
        CpMgr.inst.soaData.actStSt_AtkPhase[cpId] = AtkPhase.Windup;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.atk_GunShoot_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var soaData = CpMgr.inst.soaData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (soaData.actStSt_AtkPhase[cpId]) {
                    case AtkPhase.Windup:
                        soaData.actStSt_AtkPhase[cpId] = AtkPhase.Recovery;
                        HomingProjMgr.inst.ShootProj(
                            homingProjData,
                            hitEffects,
                            projSpawnPose.position,
                            projSpawnPose.forward,
                            homingProjTgt
                        );
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
                        Debug.LogError($"Switch defaulted with {soaData.actStSt_AtkPhase[cpId]}");
                        break;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        CpUtils.UpdateMovInputData(
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
