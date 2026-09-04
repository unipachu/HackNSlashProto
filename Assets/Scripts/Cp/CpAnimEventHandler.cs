using System;
using Unity.Mathematics;
using UnityEngine;

// TODO: Rename. There is already AnimEvent, AnimEvents, and AnimEventPlr. Confusing much?
public class CpAnimEventHandler : MonoBehaviour {
    public Action<int, CpAnimEventT> animEvent;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    // TODO: Combine similar functionality into one general event. Would it be possible to have the
    // TODO C: animation events to point more directly into some methods so that this switch case would
    // TODO C: not be needed?
    void OnAnimEvent(int id, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        CpMgr ccMgr = CpMgr.inst;
        Cp_UnityComps unityComps = ccMgr.unityComps[id];
        Cp_BaseData data = ccMgr.data;
        switch (animEvent) {
            case CpAnimEventT.Atk_GunShoot_Recovery_Finished:
                data.isAffectedByGravity[id] = false;
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Atk_GunShoot_Windup_Finished:
                // TODO: Make into So data
                HomingProjMovData projData;
                AtkData atkData = new(10, KnockbackT.Weak, 0.5f);
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
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CpAnimInfo.atk_GunShoot_Recovery
                );
                break;
            case CpAnimEventT.Atk_FlyingAtk_Windup_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CpAnimInfo.atk_FlyingAtk_Impact
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Impact;
                break;
            case CpAnimEventT.Atk_FlyingAtk_Impact_HitDealerActivated:
                // TODO: Item
                //unityComps.rHandItem.aoeHitDealer.atkData = new(1, KnockbackT.Weak, 5);
                //unityComps.rHandItem.aoeHitDealer.Activate();
                break;
            case CpAnimEventT.FlyingAtk_Impact_Finished:
                data.isAffectedByGravity[id] = true;
                data.actStSt_ImpactFinished[id] = true;
                // TODO: Set this in base data.
                data.vel_Ver[id] = -40;
                break;
            case CpAnimEventT.Atk_FlyingAtk_Recovery_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                return;
            case CpAnimEventT.Atk_HorSlash1_Impact_ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash1_Impact_ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CpAnimEventT.Atk_HorSlash1_Impact_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CpAnimInfo.atk_HorSlash1_Recovery
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                break;
            case CpAnimEventT.Atk_HorSlash1_Impact_HitDealerActivated:
                //Debug.Log($"rHandEquippable null: {unityComps.rHandEquippable == null}");
                // TODO: Item

                //unityComps.rHandItem.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                //unityComps.rHandItem.hitDealer.hitWldDir = unityComps.trf.forward;
                //unityComps.rHandItem.hitDealer.Activate();
                break;
            case CpAnimEventT.Atk_HorSlash1_Impact_HitDealerDeactivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.Deactivate();
                break;
            case CpAnimEventT.Atk_HorSlash1_Impact_RotationAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash1_Impact_RotationDisallowed:
                data.actStSt_ImpactInputRotAllowed[id] = false;
                break;
            case CpAnimEventT.Atk_HorSlash1_Recovery_DodgeAllowed:
                data.actStSt_DodgeAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash1_Recovery_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Atk_HorSlash1_Windup_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CpAnimInfo.atk_HorSlash1_Impact
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Impact;
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CpAnimInfo.atk_HorSlash2_Recovery
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_HitDealerActivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                //unityComps.rHandItem.hitDealer.hitWldDir = unityComps.trf.forward;
                //unityComps.rHandItem.hitDealer.Activate();
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_HitDealerDeactivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.Deactivate();
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_RotationAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash2_Impact_RotationDisallowed:
                data.actStSt_ImpactInputRotAllowed[id] = false;
                break;
            case CpAnimEventT.Atk_HorSlash2_Recovery_DodgeAllowed:
                data.actStSt_DodgeAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash2_Recovery_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CpAnimInfo.atk_HorSlash1_Recovery
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_HitDealerActivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                //unityComps.rHandItem.hitDealer.hitWldDir = unityComps.trf.forward;
                //unityComps.rHandItem.hitDealer.Activate();
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_HitDealerDeactivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.Deactivate();
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_RotationAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CpAnimEventT.Atk_HorSlash3_Impact_RotationDisallowed:
                data.actStSt_ImpactInputRotAllowed[id] = false;
                break;
            case CpAnimEventT.Atk_JumpVerSlam_Finished:
                data.isAffectedByGravity[id] = true;
                data.vel_Ver[id] = -data.st_AtkJump_DownSpeedAfterJumpFinished[id];
                break;
            case CpAnimEventT.Atk_JumpVerSlam_HitboxActivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                //unityComps.rHandItem.hitDealer.hitWldDir = unityComps.trf.forward;
                //unityComps.rHandItem.hitDealer.Activate();
                break;
            case CpAnimEventT.Atk_JumpVerSlam_HitboxDeactivated:
                // TODO: Item

                //unityComps.rHandItem.hitDealer.Deactivate();
                break;
            case CpAnimEventT.Atk_JumpVerSlam_JumpFinished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Atk_JumpVerSlam_JumpStarted:
                data.isAffectedByGravity[id] = false;
                break;
            case CpAnimEventT.Dodge_BufferedInputStSwitchAllowed:
                data.actStSt_BufferedInputStSwitchAllowed[id] = true;
                break;
            case CpAnimEventT.Dodge_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Dodge_InvulEnd:
                data.invul[id] = false;
                break;
            case CpAnimEventT.Dodge_YawAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CpAnimEventT.FallLanding_CanSwitchSt:
                data.actStSt_DodgeAllowed[id] = true;
                break;
            case CpAnimEventT.FallLanding_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Knockback_Weak_Bwd_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            case CpAnimEventT.Knockback_Weak_Fwd_Finished:
                if (math.all(data.input_mov[id] != float2.zero))
                    ccMgr.ActSt_SwitchState(id, CpActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CpActSt.Idle);
                break;
            default:
                Debug.Log($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
