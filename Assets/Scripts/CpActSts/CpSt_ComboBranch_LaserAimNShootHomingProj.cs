using System.Collections.Generic;
using UnityEngine;

public class CpSt_ComboBranch_LaserAimNShootHomingProj : IFsmSt_Cp {
    AimLaser aimLaser;
    IComboNode comboNode;
    CpHandle cp;
    HitEffects hitEffects;
    HomingProjData homingProjData;
    Transform projSpawnPose;
    ProjT projT;

    public CpSt_ComboBranch_LaserAimNShootHomingProj(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_LaserAimNShootHomingProj Enter(
        AimLaser aimLaser,
        IComboNode comboNode,
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
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_GunShoot_AimPose),
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
        ref var cpData = ref cp.Data;
        //Dbg.Log($"Cp {cp.I} ticked windup", cp.Data.enableDbgMsgs);
        CpUtils.UpdateMovInputData(
            cp.I,
            cpData.input_mov_LastNonZero,
            cpData.animDPos,
            0,
            180, // NOTE: Yaw speed is set here. // TODO: so
            float.PositiveInfinity
        );
        aimLaser.Tick();
        if (cpData.curStDur > 1) { // TODO: so
            Transform tgt = null;
            if (cpData.classRefs.lockOnTgt != null)
                tgt = cpData.classRefs.lockOnTgt.LockOnTrf;
            HomingProjMgr.inst.ShootProj(
                cpData.action_HitSomething,
                HitDirMode.HitDealerMovDir,
                hitEffects,
                homingProjData,
                projT,
                new HashSet<IHitReceiver> { cp.hitReciever },
                tgt,
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
        }
    }
}
