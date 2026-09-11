using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Action which always leads to one of the basic action (walk, idle, fall).
/// NOTE: Recovery action cannot be interrupted by input! If that's what you want, then use some other
/// NOTE C: action e.g. BasicImpact. (6.9.2026)
/// </summary>
public class CpSt_Atk_BasicRecovery : IFsmSt_Cp {
    int cpId;

    public CpSt_Atk_BasicRecovery(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public void Exit() {}

    public CpSt_Atk_BasicRecovery Enter(AnimInfo animInfo) {
        var unityComps = CpMgr.inst.unityComps;
        CpMgr.GetAos(cpId).act_AtkPhase = AtkPhase.Recovery;
        CpMgr.GetAos(cpId).comboAllowed = false;
        CpMgr.GetAos(cpId).inputRotAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            unityComps[cpId].anim,
            animInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cpId);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var animEventPlrData = CpMgr.inst.animEventPlrData[cpId];
        // interpolate to walking speed.
        CpMgr.GetAos(cpId).act_BasicRecovery_MotInterpTimer += Time.deltaTime;
        //Debug.Log("anim nrm time: " + animEventPlrData.prevTotalNrmT);
        float interpValue = Mathf.Clamp01(animEventPlrData.prevTotalNrmT);
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetAos(cpId).input_mov,
            float3.zero,
            CpMgr.GetAos(cpId).walkMaxLinSpd * interpValue,
            CpMgr.GetAos(cpId).walkYawSpd * interpValue,
            CpMgr.GetAos(cpId).walkLinAcc
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        if (CpMgr.GetAos(cpId).dodgeAllowed) {
            if (CpInputBuffer.TryConsumeInput(
                BufferableInput.BtnE,
                ref CpMgr.GetAos(cpId).inputBuffer_BufferedInput,
                ref CpMgr.GetAos(cpId).inputBuffer_RemainingTime)
            ) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.dodge.Enter(),cpId);
                return;
            }
        }
    }
}
