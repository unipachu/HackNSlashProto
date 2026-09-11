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
        CpMgr.GetAos(cpId).act_AtkPhase = AtkPhase.Windup;
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
        var soaData = CpMgr.inst.aosData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (CpMgr.GetAos(cpId).act_AtkPhase) {
                    case AtkPhase.Windup:
                        CpMgr.GetAos(cpId).act_AtkPhase = AtkPhase.Recovery;
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
                        Debug.LogError($"Switch defaulted with {CpMgr.GetAos(cpId).act_AtkPhase}");
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
        switch (CpMgr.GetAos(cpId).act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    cpId,
                    CpMgr.GetAos(cpId).input_mov_LastNonZero,
                    CpMgr.GetAos(cpId).animDPos,
                    0,
                    180, // NOTE: Yaw speed is set here.
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovInputData(
                    cpId,
                    CpMgr.GetAos(cpId).input_mov_LastNonZero,
                    CpMgr.GetAos(cpId).animDPos,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"{cpId} Switch defaulted with {CpMgr.GetAos(cpId).act_AtkPhase}.");
                break;
        }
    }
}
