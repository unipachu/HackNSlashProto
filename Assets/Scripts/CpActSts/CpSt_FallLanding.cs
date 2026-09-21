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
        CpMgr.GetAos(cp.Id).dodgeAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.fallLanding),
            0.2f
        );
        return this;
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.Id];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.Id))
            return;
        CpUtils.UpdateMovInputData(
            cp.Id,
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (CpMgr.GetAos(cp.Id).dodgeAllowed) {
            if (
                InputBufferUtils.TryConsumeInput(
                    BufferableInput.BtnE,
                    ref CpMgr.GetAos(cp.Id).inputBuffer_BufferedInput,
                    ref CpMgr.GetAos(cp.Id).inputBuffer_RemainingTime
                )
            ) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.dodge.Enter(), cp.Id);
                return;
            }
        }
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cp.Id);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }
}
