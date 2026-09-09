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
        Cp_SoaData soaData = CpMgr.inst.soaData;
        soaData.invul[cpId] = true;
        soaData.isAffectedByGravity[cpId] = false;
        soaData.actStSt_AtkPhase[cpId] = AtkPhase.Windup;
        CpInputBuffer.Clear(
            cpId,
            soaData.inputBuffer_BufferedInput,
            soaData.inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.atk_FlyingAtk_Windup),
            0.1f
        );
        return this;
    }

    public void Exit() {
        Cp_SoaData data = CpMgr.inst.soaData;
        data.invul[cpId] = false;
        data.isAffectedByGravity[cpId] = true;
        hitDealer.Deactivate();
    }

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        Cp_UnityComps unityComps = CpMgr.inst.unityComps[cpId];
        var aosData = CpMgr.inst.aosData[cpId];
        switch (data.actStSt_AtkPhase[cpId]) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovData(
                    cpId,
                    data,
                    data.input_mov[cpId],
                    data.animDPos[cpId],
                    aosData.st_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                break;
            case AtkPhase.Impact:
                CpUtils.UpdateMovData(
                    cpId,
                    data,
                    data.input_mov[cpId],
                    data.animDPos[cpId],
                    aosData.st_AtkFlying_TgtHorSpd,
                    0,
                    float.PositiveInfinity
                );
                if (data.isGrounded[cpId]) {
                    hitDealer.Deactivate();
                    data.actStSt_AtkPhase[cpId] = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref CpMgr.inst.animEventPlrData[cpId],
                        unityComps.anim,
                        CpAnimInfo.Get(CpAnimInfoT.atk_FlyingAtk_Recovery)
                    );
                }
                break;
            case AtkPhase.Recovery:
                CpUtils.UpdateMovData(
                    cpId,
                    data,
                    float2.zero,
                    float3.zero,
                    0,
                    0,
                    float.PositiveInfinity
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {data.actStSt_AtkPhase[cpId]}.");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var soaData = CpMgr.inst.soaData;
        var unityComps = CpMgr.inst.unityComps[cpId];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (soaData.actStSt_AtkPhase[cpId]) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cpId],
                            unityComps.anim,
                            CpAnimInfo.Get(CpAnimInfoT.atk_FlyingAtk_Impact)
                        );
                        soaData.actStSt_AtkPhase[cpId] = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        // NOTE: During impact we stop applying vertical animation root motion, and intead
                        // NOTE C: use gravity. Because of the animation logic however, the vertical movement
                        // NOTE C: caused by root motion is saved and used next tick as the starting downwards
                        // NOTE C: velocity when gravity acceleration is applied.
                        soaData.isAffectedByGravity[cpId] = true;
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cpId);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {soaData.actStSt_AtkPhase[cpId]}");
                        break;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                // TODO: below
                hitDealer.hitEffects = hitEffects;
                hitDealer.hitWldDir = unityComps.trf.forward;
                hitDealer.Activate();
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }
}

