using System.Collections.Generic;
using UnityEngine;

public class CpFlyingHeadActSt_Atk_Dash : IFsmSt_Cp{
    CpFlyingHeadHandle cpFlyingHead;
    IHitDealer hitDealer;
    HitEffects hitEffects;

    public CpFlyingHeadActSt_Atk_Dash(CpFlyingHeadHandle cpFlyingHead) {
        this.cpFlyingHead = cpFlyingHead;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpFlyingHeadActSt_Atk_Dash Enter(HitEffects hitEffects, IHitDealer hitDealer) {
        ref var commonData = ref cpFlyingHead.CommonData;
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        commonData.act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cpFlyingHead.anim,
            CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.atk_DashAtk_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        //Dbg.Log($"Animation event in {nameof(CpFlyingHeadActSt_Atk_Dash)} recieved: {animEvent}");
        ref var commonData = ref cpFlyingHead.CommonData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (commonData.act_AtkPhase) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
                            ref commonData.animEventPlrData,
                            cpFlyingHead.anim,
                            CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.atk_DashAtk_Impact)
                        );
                        commonData.act_AtkPhase = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        commonData.act_AtkPhase = AtkPhase.Recovery;
                        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
                            ref commonData.animEventPlrData,
                            cpFlyingHead.anim,
                            CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.atk_DashAtk_Recovery)
                        );
                        break;
                    case AtkPhase.Recovery:
                        CpFlyingHeadUtils.TransitionToIdleOrFly(ref commonData, ref cpFlyingHead.FlyingHeadData);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {commonData.act_AtkPhase}");
                        break;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.ResetNActivate(
                    cpFlyingHead.lockOnTrf, // NOTE: = character center point.
                    HitDirMode.FromHitSourceTrfToHitReciever,
                    hitEffects,
                    new HashSet<IHitReceiver> { cpFlyingHead.hitReciever },
                    cpFlyingHead.so_CpCommonConfig.team,
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
        ref var commonData = ref cpFlyingHead.CommonData;
        switch (commonData.act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    ref commonData,
                    commonData.input_mov,
                    commonData.animDPose.position,
                    0,
                    commonData.movInput_yawSpd,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                //Dbg.Log(
                //    $"anim d pos: {commonData.animDPose.position}",
                //    cpFlyingHead,
                //    cpFlyingHead.so_CpCommonConfig.enableDbgMsgs
                //);
                // TODO: It would feel better if the impact animation would end as it hits the player. This
                // C: state would probably need to listen to the hit reciever - or perhaps have a OnHit
                // method which CpHandle calls on Hit.
                CpUtils.UpdateMovInputData(
                    ref commonData,
                    commonData.input_mov,
                    commonData.animDPose.position,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovInputData(
                    ref commonData,
                    Vector2.zero,
                    Vector3.zero,
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
