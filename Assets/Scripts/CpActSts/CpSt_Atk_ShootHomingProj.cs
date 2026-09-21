using UnityEngine;

public class CpSt_Atk_ShootHomingProj : IFsmSt_Cp{
    // TODO: This should "combo" to windup or another shot. Currently combo node transition data is not used.
    IComboNode comboNode;
    CpHandle cp;
    HitEffects hitEffects;
    HomingProjData homingProjData;
    Transform projSpawnPose;
    Transform homingProjTgt;

    public CpSt_Atk_ShootHomingProj(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Atk_ShootHomingProj Enter(
        IComboNode comboNode,
        HitEffects hitEffects,
        HomingProjData homingProjData,
        Transform projSpawnPose,
        Transform homingProjTgt
    ) {
        this.comboNode = comboNode;
        this.hitEffects = hitEffects;
        this.homingProjData = homingProjData;
        this.projSpawnPose = projSpawnPose;
        this.homingProjTgt = homingProjTgt;
        CpMgr.GetAos(cp.Id).act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_GunShoot_Windup),
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var soaData = CpMgr.inst.aosData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (CpMgr.GetAos(cp.Id).act_AtkPhase) {
                    case AtkPhase.Windup:
                        CpMgr.GetAos(cp.Id).act_AtkPhase = AtkPhase.Recovery;
                        HomingProjMgr.inst.ShootProj(
                            homingProjData,
                            new HitData(hitEffects, soaData[cp.Id].team, projSpawnPose.forward),
                            projSpawnPose.position,
                            projSpawnPose.forward,
                            homingProjTgt
                        );
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cp.Id],
                            CpMgr.inst.unityComps[cp.Id].anim,
                            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_GunShoot_Recovery)
                        );
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cp.Id);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {CpMgr.GetAos(cp.Id).act_AtkPhase}");
                        break;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        switch (CpMgr.GetAos(cp.Id).act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    cp.Id,
                    CpMgr.GetAos(cp.Id).input_mov_LastNonZero,
                    CpMgr.GetAos(cp.Id).animDPos,
                    0,
                    180, // NOTE: Yaw speed is set here.
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovInputData(
                    cp.Id,
                    CpMgr.GetAos(cp.Id).input_mov_LastNonZero,
                    CpMgr.GetAos(cp.Id).animDPos,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"{cp.Id} Switch defaulted with {CpMgr.GetAos(cp.Id).act_AtkPhase}.");
                break;
        }
    }
}
