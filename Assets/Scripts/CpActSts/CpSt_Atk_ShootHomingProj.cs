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
        cp.Data.act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_GunShoot_Windup),
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        ref Cp_AosData cpData = ref cp.Data;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (cpData.act_AtkPhase) {
                    case AtkPhase.Windup:
                        //Dbg.Log("fired finished windup", cpData.enableDbgMsgs);
                        cpData.act_AtkPhase = AtkPhase.Recovery;
                        HomingProjMgr.inst.ShootProj(
                            homingProjData,
                            new HitData(hitEffects, cpData.team, projSpawnPose.forward),
                            projSpawnPose.position,
                            projSpawnPose.forward,
                            homingProjTgt
                        );
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cp.I],
                            CpMgr.inst.unityComps[cp.I].anim,
                            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_GunShoot_Recovery)
                        );
                        break;
                    case AtkPhase.Recovery:
                        //Dbg.Log("fired finished recovery", cpData.enableDbgMsgs);
                        CpUtils.TransitionToFallIdleOrWalk(cp.I);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {cpData.act_AtkPhase}");
                        break;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        switch (cp.Data.act_AtkPhase) {
            case AtkPhase.Windup:
                //Dbg.Log("ticked windup", CpMgr.GetAos(cp.Id).enableDbgMsgs);
                CpUtils.UpdateMovInputData(
                    cp.I,
                    cp.Data.input_mov_LastNonZero,
                    cp.Data.animDPos,
                    0,
                    180, // NOTE: Yaw speed is set here.
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Recovery:
                //Dbg.Log("ticked recovery", CpMgr.GetAos(cp.Id).enableDbgMsgs);
                CpUtils.UpdateMovInputData(
                    cp.I,
                    cp.Data.input_mov_LastNonZero,
                    cp.Data.animDPos,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"{cp.I} Switch defaulted with {cp.Data.act_AtkPhase}.");
                break;
        }
    }
}
