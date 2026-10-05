using System.Collections.Generic;
using UnityEngine;

public class CpSt_ComboBranch_ShootHomingProj : IFsmSt_Cp{
    IComboNode comboNode;
    CpHandle cp;
    HitEffects hitEffects;
    HomingProjData homingProjData;
    Transform projSpawnPose;
    ProjT projT;
    Transform homingProjTgt;

    public CpSt_ComboBranch_ShootHomingProj(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_ShootHomingProj Enter(
        IComboNode comboNode,
        HitEffects hitEffects,
        HomingProjData homingProjData,
        Transform projSpawnPose,
        ProjT projT,
        Transform homingProjTgt
    ) {
        this.comboNode = comboNode;
        this.hitEffects = hitEffects;
        this.homingProjData = homingProjData;
        this.projSpawnPose = projSpawnPose;
        this.projT = projT;
        this.homingProjTgt = homingProjTgt;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_GunShoot_Windup),
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        ref Cp_Data cpData = ref cp.Data;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                //Dbg.Log($"Cp {cp.I} fired finished windup", cpData.enableDbgMsgs);
                HomingProjMgr.inst.ShootProj(
                    cpData.action_HitSomething,
                    HitDirMode.HitDealerMovDir,
                    hitEffects,
                    homingProjData,
                    projT,
                    new HashSet<IHitReceiver>{ cp.hitReciever },
                    homingProjTgt,
                    cp.so_cpData.team,
                    projSpawnPose.position,
                    projSpawnPose.forward
                );
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.TrySwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.I),
                        cp.I,
                        true
                    );
                    return;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        //Dbg.Log($"Cp {cp.I} ticked windup", cp.Data.enableDbgMsgs);
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov_LastNonZero,
            cp.Data.animDPos,
            0,
            180, // NOTE: Yaw speed is set here. TODO: So
            float.PositiveInfinity
        );
    }
}
