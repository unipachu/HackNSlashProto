using Unity.Mathematics;
using UnityEngine;

// TODO: Rename to CpHumanoid
public class CpSt_FallLanding : IFsmSt_Cp {
    CpHumdHandle cpHumanoid;

    public CpSt_FallLanding(CpHumdHandle cpHumanoid) {
        this.cpHumanoid = cpHumanoid;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_FallLanding Enter() {
        cpHumanoid.HumdData.dodgeAllowed = false;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumanoid.CommonData.animEventPlrData,
            cpHumanoid.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.fallLanding),
            0.2f
        );
        return this;
    }

    public void Tick() {
        ref var commonData = ref cpHumanoid.CommonData;
        if (CpHumdUtils.SwitchToFallingStIfNotGrounded(ref commonData, ref cpHumanoid.HumdData))
            return;
        CpUtils.UpdateMovInputData(
            ref commonData,
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (cpHumanoid.HumdData.dodgeAllowed && cpHumanoid.HumdData.cooldownTimer_Dodge == 0) {
            if (
                InputBufferUtils.TryConsumeInput(
                    BufferableInput.BtnE,
                    ref commonData.inputBuffer_BufferedInput,
                    ref commonData.inputBuffer_RemainingTime
                )
            ) {
                CpUtils.TrySwitchActSt(
                    () => cpHumanoid.HumdData.classRefs.actSts.dodge.Enter(),
                    ref commonData,
                    true
                );
                return;
            }
        }
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpHumdUtils.TransitionToFallIdleOrWalk(ref cpHumanoid.CommonData, ref cpHumanoid.HumdData);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }
}
