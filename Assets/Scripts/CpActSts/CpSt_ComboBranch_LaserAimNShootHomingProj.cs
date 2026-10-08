using System.Collections.Generic;
using UnityEngine;

// TODO: Rename to CpHumanoidSt
public class CpSt_ComboBranch_LaserAimNShootHomingProj : IFsmSt_Cp {
    AimLaser aimLaser;
    IComboNode_CpHumanoid comboNode;
    CpHandle cpHumanoid;
    HitEffects hitEffects;
    HomingProjData homingProjData;
    Transform projSpawnPose;
    ProjT projT;

    public CpSt_ComboBranch_LaserAimNShootHomingProj(CpHandle cp) {
        this.cpHumanoid = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_LaserAimNShootHomingProj Enter(
        AimLaser aimLaser,
        IComboNode_CpHumanoid comboNode,
        HitEffects hitEffects,
        HomingProjData homingProjData,
        Transform projSpawnPose,
        ProjT projT
    ) {
        this.aimLaser = aimLaser;
        this.comboNode = comboNode;
        this.hitEffects = hitEffects;
        this.homingProjData = homingProjData;
        this.projSpawnPose = projSpawnPose;
        this.projT = projT;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumanoid.CommonData.animEventPlrData,
            cpHumanoid.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.atk_GunShoot_AimPose),
            0.1f
        );
        aimLaser.gameObject.SetActive(true);
        aimLaser.Tick();
        return this;
    }

    public void Exit() {
        aimLaser.gameObject.SetActive(false);
    }

    public void Tick() {
        ref var commonData = ref cpHumanoid.CommonData;
        //Dbg.Log($"Cp {cp.I} ticked windup", cp.Data.enableDbgMsgs);
        CpUtils.UpdateMovInputData(
            ref commonData,
            commonData.input_mov_LastNonZero,
            commonData.animDPose.position,
            0,
            cpHumanoid.so_cpCommonData.impact_YawSpd, // NOTE: Yaw speed is set here.
            float.PositiveInfinity
        );
        aimLaser.Tick();
        if (commonData.curStDur > 1) { // TODO: so
            Transform tgt = null;
            if (commonData.classRefs.lockOnTgt != null)
                tgt = commonData.classRefs.lockOnTgt.LockOnTrf;
            HomingProjMgr.inst.ShootProj(
                commonData.action_HitSomething,
                HitDirMode.HitDealerMovDir,
                hitEffects,
                homingProjData,
                projT,
                new HashSet<IHitReceiver> { cpHumanoid.hitReciever },
                tgt,
                cpHumanoid.so_cpCommonData.team,
                projSpawnPose.position,
                projSpawnPose.forward
            );
            if (comboNode.GetNextNode(BufferableInput.None) != null) {
                CpMgr.TrySwitchActSt(
                    comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpHumanoid),
                    ref cpHumanoid.CommonData,
                    true
                );
                return;
            }
        }
    }
}
