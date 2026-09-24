using System.Collections.Generic;
using UnityEngine;

public class CpSt_Atk_ShootHomingProj : IFsmSt_Cp{
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
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityObjs.anim,
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
                    homingProjData,
                    // TODO: Build hit data in the projectile (since it can change direction).
                    new HitData(
                        hitEffects,
                        cpData.team,
                        HitDirMode.WldDir,
                        null,
                        projSpawnPose.forward
                    ),
                    new HashSet<IHitReceiver>{ cp.unityObjs.hitReciever },
                    projSpawnPose.position,
                    projSpawnPose.forward,
                    homingProjTgt
                );
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.I),
                        cp.I
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
            180, // NOTE: Yaw speed is set here.
            float.PositiveInfinity
        );
        //case AtkPhase.Recovery:
        //    //Dbg.Log($"Cp {cp.I} ticked recovery", cp.Data.enableDbgMsgs);
        //    CpUtils.UpdateMovInputData(
        //        cp.I,
        //        cp.Data.input_mov_LastNonZero,
        //        cp.Data.animDPos,
        //        0,
        //        0,
        //        float.PositiveInfinity
        //    );
        //    break;
        //default:
        //    Debug.LogError($"{cp.I} Switch defaulted with {cp.Data.act_AtkPhase}.");
        //    break;
    }
}
