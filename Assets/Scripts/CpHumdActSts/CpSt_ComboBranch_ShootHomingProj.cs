using System.Collections.Generic;
using UnityEngine;

public class CpSt_ComboBranch_ShootHomingProj : IFsmSt_Cp{
    IComboNode_CpHumanoid comboNode;
    CpHumdHandle cpHumd;
    HitEffects hitEffects;
    HomingProjData homingProjData;
    Transform projSpawnPose;
    ProjT projT;
    Transform homingProjTgt;

    public CpSt_ComboBranch_ShootHomingProj(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_ShootHomingProj Enter(
        IComboNode_CpHumanoid comboNode,
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
            ref cpHumd.CommonData.animEventPlrData,
            cpHumd.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.atk_GunShoot_Windup),
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                //Dbg.Log($"Cp {cp.I} fired finished windup", cpData.enableDbgMsgs);
                HomingProjMgr.inst.ShootProj(
                    cpHumd.CommonData.action_HitSomething,
                    HitDirMode.HitDealerMovDir,
                    hitEffects,
                    homingProjData,
                    projT,
                    new HashSet<IHitReceiver>{ cpHumd.hitReciever },
                    homingProjTgt,
                    cpHumd.so_cpCommonData.team,
                    projSpawnPose.position,
                    projSpawnPose.forward
                );
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpUtils.TrySwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpHumd),
                        ref cpHumd.CommonData,
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
        ref Cp_CommonData commonData = ref cpHumd.CommonData;
        CpUtils.UpdateMovInputData(
            ref commonData,
            commonData.input_mov_LastNonZero,
            commonData.animDPose.position,
            0,
            cpHumd.so_cpCommonData.impact_YawSpd, // NOTE: Yaw speed is set here.
            float.PositiveInfinity
        );
    }
}
