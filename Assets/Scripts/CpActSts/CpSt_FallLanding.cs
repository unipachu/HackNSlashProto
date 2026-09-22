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
        CpMgr.GetData(cp.I).dodgeAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.fallLanding),
            0.2f
        );
        return this;
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.I];
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
        if (CpMgr.GetData(cp.I).dodgeAllowed) {
            if (
                InputBufferUtils.TryConsumeInput(
                    BufferableInput.BtnE,
                    ref CpMgr.GetData(cp.I).inputBuffer_BufferedInput,
                    ref CpMgr.GetData(cp.I).inputBuffer_RemainingTime
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
