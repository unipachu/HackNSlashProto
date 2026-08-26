using System;
using Unity.Mathematics;
using UnityEngine;

// TODO: Rename. There is already AnimEvent, AnimEvents, and AnimEventPlr. Confusing much?
public class CapsuleCharAnimEvents : MonoBehaviour {
    public Action<int, CapsuleCharAnimEventT> animEvent;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    // TODO: Combine similar functionality into one general event. Would it be possible to have the
    // TODO C: animation events to point more directly into some methods so that this switch case would
    // TODO C: not be needed?
    void OnAnimEvent(int id, CapsuleCharAnimEventT animEvent) {
        CapsuleCharMgr ccMgr = CapsuleCharMgr.inst;
        CapsuleChar_UnityComps unityComps = ccMgr.unityComps[id];
        CapsuleChar_BaseData data = ccMgr.data;
        switch (animEvent) {
            case CapsuleCharAnimEventT.Atk_GunShoot_Recovery_Finished:
                data.isAffectedByGravity[id] = false;
                if (!data.input_mov.Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Atk_GunShoot_Windup_Finished:
                // TODO: Make into So data
                HomingProjMovData projData;
                AtkData atkData = new(10, KnockbackT.Weak, 0.5f);
                float spd = 5;
                float maxLifetime = 10;
                float homingStr = 2;
                projData = new(spd, maxLifetime, homingStr);
                HomingProjMgr.inst.ShootProj(
                    projData,
                    atkData,
                    unityComps.rHandEquippable.projSpawnPose.position,
                    unityComps.rHandEquippable.projSpawnPose.forward,
                    // TODO: Set homing target.
                    null
                );
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CapsuleCharAnimInfo.atk_GunShoot_Recovery,
                    unityComps.animEvents.animEvent
                );
                break;
            case CapsuleCharAnimEventT.Atk_FlyingAtk_Windup_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CapsuleCharAnimInfo.atk_FlyingAtk_Impact,
                    unityComps.animEvents.animEvent
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Impact;
                break;
            case CapsuleCharAnimEventT.Atk_FlyingAtk_Impact_HitDealerActivated:
                unityComps.rHandEquippable.aoeHitDealer.atkData = new(1, KnockbackT.Weak, 5);
                unityComps.rHandEquippable.aoeHitDealer.Activate();
                break;
            case CapsuleCharAnimEventT.FlyingAtk_Impact_Finished:
                data.isAffectedByGravity[id] = true;
                data.actStSt_ImpactFinished[id] = true;
                // TODO: Set this in base data.
                data.vel_Ver[id] = -40;
                break;
            case CapsuleCharAnimEventT.Atk_FlyingAtk_Recovery_Finished:
                if (!data.input_mov.Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                return;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
                    unityComps.animEvents.animEvent
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_HitDealerActivated:
                unityComps.rHandEquippable.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                unityComps.rHandEquippable.hitDealer.hitWldDir = unityComps.transform.forward;
                unityComps.rHandEquippable.hitDealer.Activate();
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_HitDealerDeactivated:
                unityComps.rHandEquippable.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_RotationAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Impact_RotationDisallowed:
                data.actStSt_ImpactInputRotAllowed[id] = false;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Recovery_DodgeAllowed:
                data.actStSt_DodgeAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Recovery_Finished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash1_Windup_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Impact,
                    unityComps.animEvents.animEvent
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Impact;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CapsuleCharAnimInfo.atk_HorSlash2_Recovery,
                    unityComps.animEvents.animEvent
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_HitDealerActivated:
                unityComps.rHandEquippable.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                unityComps.rHandEquippable.hitDealer.hitWldDir = unityComps.transform.forward;
                unityComps.rHandEquippable.hitDealer.Activate();
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_HitDealerDeactivated:
                unityComps.rHandEquippable.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_RotationAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Impact_RotationDisallowed:
                data.actStSt_ImpactInputRotAllowed[id] = false;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Recovery_DodgeAllowed:
                data.actStSt_DodgeAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash2_Recovery_Finished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_Finished:
                AnimEventPlr.CrossfadeNInitAnimEventPlr(
                    ref ccMgr.animEventPlrData[id],
                    unityComps.anim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
                    unityComps.animEvents.animEvent
                );
                data.actStSt_AtkPhase[id] = AtkPhase.Recovery;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_HitDealerActivated:
                unityComps.rHandEquippable.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                unityComps.rHandEquippable.hitDealer.hitWldDir = unityComps.transform.forward;
                unityComps.rHandEquippable.hitDealer.Activate();
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_HitDealerDeactivated:
                unityComps.rHandEquippable.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_RotationAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Atk_HorSlash3_Impact_RotationDisallowed:
                data.actStSt_ImpactInputRotAllowed[id] = false;
                break;
            case CapsuleCharAnimEventT.Atk_JumpVerSlam_Finished:
                data.isAffectedByGravity[id] = true;
                data.vel_Ver[id] = -data.st_AtkJump_DownSpeedAfterJumpFinished[id];
                break;
            case CapsuleCharAnimEventT.Atk_JumpVerSlam_HitboxActivated:
                unityComps.rHandEquippable.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                unityComps.rHandEquippable.hitDealer.hitWldDir = unityComps.transform.forward;
                unityComps.rHandEquippable.hitDealer.Activate();
                break;
            case CapsuleCharAnimEventT.Atk_JumpVerSlam_HitboxDeactivated:
                unityComps.rHandEquippable.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEventT.Atk_JumpVerSlam_JumpFinished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Atk_JumpVerSlam_JumpStarted:
                data.isAffectedByGravity[id] = false;
                break;
            case CapsuleCharAnimEventT.Dodge_BufferedInputStSwitchAllowed:
                data.actStSt_BufferedInputStSwitchAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.Dodge_Finished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Dodge_InvulEnd:
                data.invul[id] = false;
                break;
            case CapsuleCharAnimEventT.Dodge_YawAllowed:
                data.actStSt_ImpactInputRotAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.FallLanding_CanSwitchSt:
                data.actStSt_DodgeAllowed[id] = true;
                break;
            case CapsuleCharAnimEventT.FallLanding_Finished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Knockback_Weak_Bwd_Finished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            case CapsuleCharAnimEventT.Knockback_Weak_Fwd_Finished:
                if (!data.input_mov[id].Equals(float2.zero))
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Walk);
                else
                    ccMgr.ActSt_SwitchState(id, CapsuleCharActSt.Idle);
                break;
            default:
                Debug.Log($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
