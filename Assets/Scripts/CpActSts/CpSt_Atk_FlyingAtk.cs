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
        CpMgr.GetData(cp.I).ignoreHits = true;
        CpMgr.GetData(cp.I).isAffectedByGravity = false;
        CpMgr.GetData(cp.I).act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_FlyingAtk_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetData(cp.I).ignoreHits = false;
        CpMgr.GetData(cp.I).isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var aos = CpMgr.inst.aosData;
        var unityComps = CpMgr.inst.unityComps[cp.I];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (CpMgr.GetData(cp.I).act_AtkPhase) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cp.I],
                            unityComps.anim,
                            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_FlyingAtk_Impact)
                        );
                        CpMgr.GetData(cp.I).act_AtkPhase = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        // NOTE: During impact we stop applying vertical animation root motion, and intead
                        // NOTE C: use gravity. Because of the animation logic however, the vertical movement
                        // NOTE C: caused by root motion is saved and used next tick as the starting downwards
                        // NOTE C: velocity when gravity acceleration is applied.
                        CpMgr.GetData(cp.I).isAffectedByGravity = true;
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cp.I);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {CpMgr.GetData(cp.I).act_AtkPhase}");
                        break;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.hitData = new HitData(hitEffects, aos[cp.I].team, cp.transform.forward);
                hitDealer.Activate();
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        Cp_UnityObjs unityComps = CpMgr.inst.unityComps[cp.I];
        switch (CpMgr.GetData(cp.I).act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    cp.I,
                    CpMgr.GetData(cp.I).input_mov,
                    CpMgr.GetData(cp.I).animDPos,
                    CpMgr.GetData(cp.I).act_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CpUtils.UpdateMovInputData(
                    cp.I,
                    CpMgr.GetData(cp.I).input_mov,
                    CpMgr.GetData(cp.I).animDPos,
                    CpMgr.GetData(cp.I).act_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                if (CpMgr.GetData(cp.I).isGrounded) {
                    hitDealer.Deactivate();
                    CpMgr.GetData(cp.I).act_AtkPhase = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref CpMgr.inst.animEventPlrData[cp.I],
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
                Debug.LogError($"Switch defaulted with {CpMgr.GetData(cp.I).act_AtkPhase}.");
                break;
        }
    }
}

