using Unity.Mathematics;
using UnityEngine;

public class CpSt_FallLanding : IFsmSt_Cp {
    int cpId;

    public CpSt_FallLanding(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_FallLanding Enter() {
        CpMgr.GetAos(cpId).dodgeAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.fallLanding),
            0.2f
        );
        return this;
    }

    public void Exit() {}

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        CpUtils.UpdateMovInputData(
            cpId,
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (CpMgr.GetAos(cpId).dodgeAllowed) {
            if (
                CpInputBuffer.TryConsumeInput(
                    BufferableInput.BtnE,
                    ref CpMgr.GetAos(cpId).inputBuffer_BufferedInput,
                    ref CpMgr.GetAos(cpId).inputBuffer_RemainingTime
                )
            ) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.dodge.Enter(), cpId);
                return;
            }
        }
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
}
