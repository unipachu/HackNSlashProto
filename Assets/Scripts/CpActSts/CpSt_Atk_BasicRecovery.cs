using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Action which always leads to one of the basic action (walk, idle, fall).
/// NOTE: Recovery action cannot be interrupted by input! If that's what you want, then use some other
/// NOTE C: action e.g. BasicImpact. (6.9.2026)
/// </summary>
public class CpSt_Atk_BasicRecovery : IFsmSt_Cp {
    CpHandle cp;
    /// <summary>
    /// Normalized animation time when <see cref="Cp_Data.inputMovAllowed"/> is first read as true in Tick().
    /// </summary>
    float inputMovAllowedNrmTime;

    public CpSt_Atk_BasicRecovery(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Atk_BasicRecovery Enter(AnimInfo animInfo) {
        ref var cpData = ref cp.Data;
        inputMovAllowedNrmTime = -1;
        cpData.comboAllowed = false;
        cpData.inputMovAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref cpData.animEventPlrData,
            cpData.unityObjs.anim,
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
        ref var cpData = ref cp.Data;
        //Debug.Log("anim nrm time: " + animEventPlrData.prevTotalNrmT);
        float interpValue = 0;
        if (cpData.inputMovAllowed) {
            if (inputMovAllowedNrmTime < 0)
                inputMovAllowedNrmTime = cpData.animEventPlrData.prevTotalNrmT;
            // Interpolate to walking speed.
            interpValue = (cpData.animEventPlrData.prevTotalNrmT - inputMovAllowedNrmTime)
                / (1 - inputMovAllowedNrmTime);
            interpValue = Mathf.Clamp01(interpValue);
        }
        //Dbg.Log($"{nameof(interpValue)}: {interpValue}", cp, cpData.enableDbgMsgs);
        CpUtils.UpdateMovInputData(
            cp.I,
            cpData.input_mov,
            float3.zero,
            cpData.walkMaxLinSpd * interpValue,
            cpData.walkYawSpd * interpValue,
            cpData.walkLinAcc
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cpData.dodgeAllowed) {
            if (InputBufferUtils.TryConsumeInput(
                BufferableInput.BtnE,
                ref cpData.inputBuffer_BufferedInput,
                ref cpData.inputBuffer_RemainingTime)
            ) {
                CpMgr.inst.SwitchActSt(() => CpMgr.inst.aos[cp.I].classRefs.actSts.dodge.Enter(), cp.I);
                return;
            }
        }
    }
}
