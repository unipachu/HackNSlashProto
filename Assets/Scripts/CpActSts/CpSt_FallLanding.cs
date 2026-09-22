using Unity.Mathematics;
using UnityEngine;

public class CpSt_FallLanding : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_FallLanding(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_FallLanding Enter() {
        cp.Data.dodgeAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityComps.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.fallLanding),
            0.2f
        );
        return this;
    }

    public void Tick() {
        var classRefs = cp.Data.classRefs;
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        CpUtils.UpdateMovInputData(
            cp.I,
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (cp.Data.dodgeAllowed) {
            if (
                InputBufferUtils.TryConsumeInput(
                    BufferableInput.BtnE,
                    ref cp.Data.inputBuffer_BufferedInput,
                    ref cp.Data.inputBuffer_RemainingTime
                )
            ) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.dodge.Enter(), cp.I);
                return;
            }
        }
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
}
