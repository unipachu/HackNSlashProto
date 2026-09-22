using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Action which always leads to one of the basic action (walk, idle, fall).
/// NOTE: Recovery action cannot be interrupted by input! If that's what you want, then use some other
/// NOTE C: action e.g. BasicImpact. (6.9.2026)
/// </summary>
public class CpSt_Atk_BasicRecovery : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Atk_BasicRecovery(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Atk_BasicRecovery Enter(AnimInfo animInfo) {
        var unityComps = CpMgr.inst.unityComps;
        CpMgr.GetData(cp.I).act_AtkPhase = AtkPhase.Recovery;
        CpMgr.GetData(cp.I).comboAllowed = false;
        CpMgr.GetData(cp.I).inputRotAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            unityComps[cp.I].anim,
            animInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cp.I);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.I];
        var animEventPlrData = CpMgr.inst.animEventPlrData[cp.I];
        // interpolate to walking speed.
        CpMgr.GetData(cp.I).act_BasicRecovery_MotInterpTimer += Time.deltaTime;
        //Debug.Log("anim nrm time: " + animEventPlrData.prevTotalNrmT);
        float interpValue = Mathf.Clamp01(animEventPlrData.prevTotalNrmT);
        CpUtils.UpdateMovInputData(
            cp.I,
            CpMgr.GetData(cp.I).input_mov,
            float3.zero,
            CpMgr.GetData(cp.I).walkMaxLinSpd * interpValue,
            CpMgr.GetData(cp.I).walkYawSpd * interpValue,
            CpMgr.GetData(cp.I).walkLinAcc
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (CpMgr.GetData(cp.I).dodgeAllowed) {
            if (InputBufferUtils.TryConsumeInput(
                BufferableInput.BtnE,
                ref CpMgr.GetData(cp.I).inputBuffer_BufferedInput,
                ref CpMgr.GetData(cp.I).inputBuffer_RemainingTime)
            ) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.dodge.Enter(),cp.I);
                return;
            }
        }
    }
}
