using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_FlyingAtk : IFsmSt_Cp {
    CpHandle cp;
    HitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_FlyingAtk(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt 
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_FlyingAtk Enter(HitEffects hitEffects, HitDealer hitDealer) {
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        cp.Data.ignoreHits = true;
        cp.Data.isAffectedByGravity = false;
        cp.Data.act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityObjs.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_FlyingAtk_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
        cp.Data.ignoreHits = false;
        cp.Data.isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var cpData = cp.Data;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (cp.Data.act_AtkPhase) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.aos[cp.I].animEventPlrData,
                            cpData.unityObjs.anim,
                            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_FlyingAtk_Impact)
                        );
                        cp.Data.act_AtkPhase = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        // NOTE: During impact we stop applying vertical animation root motion, and intead
                        // NOTE C: use gravity. Because of the animation logic however, the vertical movement
                        // NOTE C: caused by root motion is saved and used next tick as the starting downwards
                        // NOTE C: velocity when gravity acceleration is applied.
                        cp.Data.isAffectedByGravity = true;
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cp.I);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {cp.Data.act_AtkPhase}");
                        break;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.Activate(
                    cp.unityObjs.lockOnTrf, // NOTE: = character center point.
                    HitDirMode.FromHitSourceTrfToHitReciever,
                    hitEffects,
                    new HashSet<IHitReceiver> { cp.unityObjs.hitReciever },
                    cp.Data.team
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        Cp_UnityObjs unityComps = CpMgr.inst.aos[cp.I].unityObjs;
        switch (cp.Data.act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    cp.I,
                    cp.Data.input_mov,
                    cp.Data.animDPos,
                    cp.Data.act_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CpUtils.UpdateMovInputData(
                    cp.I,
                    cp.Data.input_mov,
                    cp.Data.animDPos,
                    cp.Data.act_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                if (cp.Data.isGrounded) {
                    hitDealer.Deactivate();
                    cp.Data.act_AtkPhase = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref CpMgr.inst.aos[cp.I].animEventPlrData,
                        unityComps.anim,
                        CpAnimInfoFactory.Construct(CpAnimInfoT.atk_FlyingAtk_Recovery)
                    );
                }
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovInputData(
                    cp.I,
                    float2.zero,
                    float3.zero,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {cp.Data.act_AtkPhase}.");
                break;
        }
    }
}

