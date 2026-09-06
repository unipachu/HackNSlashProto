using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_FlyingAtk : IFsmSt_Cp {
    int cpId;
    HitDealer hitDealer;

    public CpSt_Atk_FlyingAtk(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt 
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_FlyingAtk Enter(HitDealer hitDealer) {
        this.hitDealer = hitDealer;
        Cp_SoaData data = CpMgr.inst.soaData;
        data.invul[cpId] = true;
        data.isAffectedByGravity[cpId] = false;
        data.actStSt_AtkPhase[cpId] = AtkPhase.Windup;
        data.actStSt_ImpactFinished[cpId] = false;
        CpInputBuffer.Clear(
            cpId,
            data.inputBuffer_BufferedInput,
            data.inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.atk_FlyingAtk_Windup,
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
        Cp_UnityComps[] unityComps = CpMgr.inst.unityComps;
        switch (data.actStSt_AtkPhase[cpId]) {
            case AtkPhase.Windup:
                CpUtils.UpdateMovData(
                    cpId,
                    data,
                    data.input_mov[cpId],
                    data.animDPos[cpId],
                    2, // TODO: To So field
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
                    2, // TODO: To So parameter.
                    0,
                    float.PositiveInfinity
                );
                if (data.isGrounded[cpId] && data.actStSt_ImpactFinished[cpId]) {
                    hitDealer.Deactivate();
                    data.actStSt_AtkPhase[cpId] = AtkPhase.Recovery;
                    AnimEventPlr.CrossfadeNInitAnimEventPlr(
                        ref CpMgr.inst.animEventPlrData[cpId],
                        unityComps[cpId].anim,
                        CpAnimInfo.atk_FlyingAtk_Recovery
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

    public void LateTick() {
    }

    public void PhysicsTick() {
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var data = CpMgr.inst.soaData;
        var classRefs = CpMgr.inst.unityComps[cpId];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                switch (data.actStSt_AtkPhase[cpId]) {
                    case AtkPhase.Windup:
                        AnimEventPlr.CrossfadeNInitAnimEventPlr(
                            ref CpMgr.inst.animEventPlrData[cpId],
                            classRefs.anim,
                            CpAnimInfo.atk_FlyingAtk_Impact
                        );
                        data.actStSt_AtkPhase[cpId] = AtkPhase.Impact;
                        break;
                    case AtkPhase.Impact:
                        data.isAffectedByGravity[cpId] = true;
                        data.actStSt_ImpactFinished[cpId] = true;
                        // TODO: Set this in base data.
                        data.vel_Ver[cpId] = -40;
                        break;
                    case AtkPhase.Recovery:
                        CpUtils.TransitionToFallIdleOrWalk(cpId);
                        break;
                    default:
                        Debug.LogError($"Switch defaulted with {data.actStSt_AtkPhase[cpId]}");
                        break;
                }
                CpUtils.TransitionToFallIdleOrWalk(cpId);
                break;
            case CpAnimEventT.BufferedInputStSwitchAllowed:
                break;
            case CpAnimEventT.ComboAllowed:
                break;
            case CpAnimEventT.ComboDisallowed:
                break;
            case CpAnimEventT.DodgeAllowed:
                break;
            case CpAnimEventT.HitDealerActivated:
                // TODO: Item
                //unityComps.rHandItem.aoeHitDealer.atkData = new(1, KnockbackT.Weak, 5);
                //unityComps.rHandItem.aoeHitDealer.Activate();
                break;
            case CpAnimEventT.HitDealerDeactivated:
                break;
            case CpAnimEventT.InvulEnd:
                break;
            case CpAnimEventT.AirtimeEnded:
                break;
            case CpAnimEventT.AirtimeStarted:
                break;
            case CpAnimEventT.YawDisallowed:
                break;
            case CpAnimEventT.YawAllowed:
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }
}

