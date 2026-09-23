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
        var unityComps = cp.Data.unityObjs;
        cp.Data.act_AtkPhase = AtkPhase.Recovery;
        cp.Data.comboAllowed = false;
        cp.Data.inputRotAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.Data.unityObjs.anim,
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
        var classRefs = cp.Data.classRefs;
        var animEventPlrData = CpMgr.inst.aos[cp.I].animEventPlrData;
        // interpolate to walking speed.
        cp.Data.act_BasicRecovery_MotInterpTimer += Time.deltaTime;
        //Debug.Log("anim nrm time: " + animEventPlrData.prevTotalNrmT);
        float interpValue = Mathf.Clamp01(animEventPlrData.prevTotalNrmT);
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov,
            float3.zero,
            cp.Data.walkMaxLinSpd * interpValue,
            cp.Data.walkYawSpd * interpValue,
            cp.Data.walkLinAcc
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cp.Data.dodgeAllowed) {
            if (InputBufferUtils.TryConsumeInput(
                BufferableInput.BtnE,
                ref cp.Data.inputBuffer_BufferedInput,
                ref cp.Data.inputBuffer_RemainingTime)
            ) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.dodge.Enter(),cp.I);
                return;
            }
        }
    }
}
