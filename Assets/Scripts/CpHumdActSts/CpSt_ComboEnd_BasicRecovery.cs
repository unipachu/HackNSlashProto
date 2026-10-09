using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Action which always leads to one of the basic action (walk, idle, fall).
/// NOTE: Recovery action cannot be interrupted by input! If that's what you want, then use some other
/// NOTE C: action e.g. BasicImpact. (6.9.2026)
/// </summary>
public class CpSt_ComboEnd_BasicRecovery : IFsmSt_Cp {
    CpHumdHandle cpHumd;
    /// <summary>
    /// Normalized animation time when <see cref="CpHumd_Data.inputMovAllowed"/> is first read as true in Tick().
    /// </summary>
    float inputMovAllowedNrmTime;

    public CpSt_ComboEnd_BasicRecovery(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboEnd_BasicRecovery Enter(AnimInfo animInfo) {
        ref var commonData = ref cpHumd.CommonData;
        inputMovAllowedNrmTime = -1;
        commonData.comboAllowed = false;
        commonData.inputMovAllowed = false;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cpHumd.anim,
            animInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpHumdUtils.TransitionToFallIdleOrWalk(ref cpHumd.CommonData, ref cpHumd.HumdData);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        ref var commonData = ref cpHumd.CommonData;
        ref var humdData = ref cpHumd.HumdData;
        //Debug.Log("anim nrm time: " + animEventPlrData.prevTotalNrmT);
        float interpValue = 0;
        if (commonData.inputMovAllowed) {
            if (inputMovAllowedNrmTime < 0)
                inputMovAllowedNrmTime = commonData.animEventPlrData.prevTotalNrmT;
            // Interpolate to walking speed.
            interpValue = (commonData.animEventPlrData.prevTotalNrmT - inputMovAllowedNrmTime)
                / (1 - inputMovAllowedNrmTime);
            interpValue = Mathf.Clamp01(interpValue);
        }
        //Dbg.Log($"{nameof(interpValue)}: {interpValue}", cp, cpData.enableDbgMsgs);
        CpUtils.UpdateMovInputData(
            ref commonData,
            commonData.input_mov,
            float3.zero,
            cpHumd.so_cpCommonData.walkTgtHorSpd * interpValue,
            cpHumd.so_cpCommonData.walkYawSpd * interpValue,
            cpHumd.so_cpCommonData.walkHorAcc
        );
        if (CpHumdUtils.SwitchToFallingStIfNotGrounded(ref commonData, ref cpHumd.HumdData))
            return;
        if (humdData.dodgeAllowed && humdData.cooldownTimer_Dodge == 0) {
            if (InputBufferUtils.TryConsumeInput(
                BufferableInput.B,
                ref commonData.inputBuffer_BufferedInput,
                ref commonData.inputBuffer_RemainingTime)
            ) {
                CpUtils.TrySwitchActSt(
                    () => CpHumdMgr.inst.humdData[cpHumd.I].classRefs.actSts.dodge.Enter(),
                    ref commonData,
                    true
                );
                return;
            }
        }
    }
}
