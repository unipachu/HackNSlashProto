using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_FlyingAtk : IFsmSt_Cp {
    CpHumdHandle cpHumd;
    IHitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_FlyingAtk(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt 
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_FlyingAtk Enter(HitEffects hitEffects, IHitDealer hitDealer) {
        ref var commonData = ref cpHumd.CommonData;
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        commonData.ignoreHits = true;
        commonData.isAffectedByGravity = false;
        commonData.act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cpHumd.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.atk_FlyingAtk_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
        cpHumd.CommonData.ignoreHits = false;
        cpHumd.CommonData.isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        ref var commonData = ref cpHumd.CommonData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (commonData.act_AtkPhase) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
                            ref commonData.animEventPlrData,
                            cpHumd.anim,
                            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.atk_FlyingAtk_Impact)
                        );
                        commonData.act_AtkPhase = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        // NOTE: During impact we stop applying vertical animation root motion, and intead
                        // NOTE C: use gravity. Because of the animation logic however, the vertical movement
                        // NOTE C: caused by root motion is saved and used next tick as the starting downwards
                        // NOTE C: velocity when gravity acceleration is applied.
                        commonData.isAffectedByGravity = true;
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(ref commonData, ref cpHumd.HumdData);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {commonData.act_AtkPhase}");
                        break;
                }
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
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        ref var commonData = ref cpHumd.CommonData;
        switch (commonData.act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    ref commonData,
                    commonData.input_mov,
                    commonData.animDPose.position,
                    cpHumd.so_cpHumdConfig.act_AtkFlying_TgtHorSpeed,
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CpUtils.UpdateMovInputData(
                    ref commonData,
                    commonData.input_mov,
                    commonData.animDPose.position,
                    cpHumd.so_cpHumdConfig.act_AtkFlying_TgtHorSpeed,
                    0,
                    float.PositiveInfinity
                );
                if (commonData.isGrounded) {
                    hitDealer.Deactivate();
                    commonData.act_AtkPhase = AtkPhase.Recovery;
                    AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
                        ref commonData.animEventPlrData,
                        cpHumd.anim,
                        CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.atk_FlyingAtk_Recovery)
                    );
                }
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovInputData(
                    ref commonData,
                    float2.zero,
                    float3.zero,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {commonData.act_AtkPhase}.");
                break;
        }
    }
}

