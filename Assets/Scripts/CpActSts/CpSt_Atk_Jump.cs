using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_Jump : IFsmSt_Cp {
    CpHandle cpHumd;
    IHitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_Jump(CpHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_Jump Enter(HitEffects hitEffects, IHitDealer hitDealer) {
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumd.CommonData.animEventPlrData,
            cpHumd.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.atk_JumpVerSlam),
            0.1f
        );
        return this;
    }

    public void Exit() {
        cpHumd.CommonData.isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.AirtimeEnded:
                cpHumd.CommonData.isAffectedByGravity = true;
                cpHumd.CommonData.vel_Ver = -cpHumd.so_cpHumdConfig.act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                cpHumd.CommonData.isAffectedByGravity = false;
                break;
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(ref cpHumd.CommonData, ref cpHumd.HumdData);
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.ResetNActivate(
                    cpHumd.lockOnTrf, // NOTE: = character center point.
                    HitDirMode.FromHitSourceTrfToHitReciever,
                    hitEffects,
                    new HashSet<IHitReceiver> { cpHumd.hitReciever },
                    cpHumd.so_cpCommonData.team,
                    Vector3.zero
                );
                break;
            case CpAnimEventT.HitDealerDeactivated:
                hitDealer.Deactivate();
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        CpUtils.UpdateMovInputData(
            ref cpHumd.CommonData,
            float2.zero,
            cpHumd.CommonData.animDPose.position,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
