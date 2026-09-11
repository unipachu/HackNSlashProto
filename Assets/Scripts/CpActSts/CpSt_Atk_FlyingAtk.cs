using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_FlyingAtk : IFsmSt_Cp {
    int cpId;
    HitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_FlyingAtk(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt 
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_FlyingAtk Enter(HitEffects hitEffects, HitDealer hitDealer) {
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        CpMgr.GetAos(cpId).invul = true;
        CpMgr.GetAos(cpId).isAffectedByGravity = false;
        CpMgr.GetAos(cpId).act_AtkPhase = AtkPhase.Windup;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.atk_FlyingAtk_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetAos(cpId).invul = false;
        CpMgr.GetAos(cpId).isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var soaData = CpMgr.inst.aosData;
        var unityComps = CpMgr.inst.unityComps[cpId];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (CpMgr.GetAos(cpId).act_AtkPhase) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cpId],
                            unityComps.anim,
                            CpAnimInfo.Get(CpAnimInfoT.atk_FlyingAtk_Impact)
                        );
                        CpMgr.GetAos(cpId).act_AtkPhase = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        // NOTE: During impact we stop applying vertical animation root motion, and intead
                        // NOTE C: use gravity. Because of the animation logic however, the vertical movement
                        // NOTE C: caused by root motion is saved and used next tick as the starting downwards
                        // NOTE C: velocity when gravity acceleration is applied.
                        CpMgr.GetAos(cpId).isAffectedByGravity = true;
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cpId);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {CpMgr.GetAos(cpId).act_AtkPhase}");
                        break;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.hitEffects = hitEffects;
                hitDealer.hitWldDir = unityComps.rootTrf.forward;
                hitDealer.Activate();
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        Cp_UnityComps unityComps = CpMgr.inst.unityComps[cpId];
        switch (CpMgr.GetAos(cpId).act_AtkPhase) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovInputData(
                    cpId,
                    CpMgr.GetAos(cpId).input_mov,
                    CpMgr.GetAos(cpId).animDPos,
                    CpMgr.GetAos(cpId).act_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CpUtils.UpdateMovInputData(
                    cpId,
                    CpMgr.GetAos(cpId).input_mov,
                    CpMgr.GetAos(cpId).animDPos,
                    CpMgr.GetAos(cpId).act_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                if (CpMgr.GetAos(cpId).isGrounded) {
                    hitDealer.Deactivate();
                    CpMgr.GetAos(cpId).act_AtkPhase = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref CpMgr.inst.animEventPlrData[cpId],
                        unityComps.anim,
                        CpAnimInfo.Get(CpAnimInfoT.atk_FlyingAtk_Recovery)
                    );
                }
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovInputData(
                    cpId,
                    float2.zero,
                    float3.zero,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {CpMgr.GetAos(cpId).act_AtkPhase}.");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}
}

