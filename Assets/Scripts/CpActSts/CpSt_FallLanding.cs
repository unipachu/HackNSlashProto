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
        Cp_SoaData data = CpMgr.inst.soaData;
        data.actStSt_DodgeAllowed[cpId] = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.fallLanding,
            // TODO MINOR: You could make this transition faster if falling from higher/faster,
            // TODO MINOR C: e.g. 0.2 if hitting the ground with slow speed, and 0.1 if hitting
            // TODO MINOR C: the ground while fast falling speed.
            0.2f
        );
        return this;
    }

    public void Exit() {}

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        Cp_SoaData soaData = CpMgr.inst.soaData;
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        CpUtils.UpdateMovData(
            cpId,
            soaData,
            float2.zero,
            float3.zero,
            0,
            0,
            float.PositiveInfinity
        );
        if (soaData.actStSt_DodgeAllowed[cpId]) {
            if (
                CpInputBuffer.TryConsumeInput(
                    cpId, BufferableInput.BtnE, 
                    soaData.inputBuffer_BufferedInput, 
                    soaData.inputBuffer_RemainingTime
                )
            ) {
                CpMgr.inst.SwitchToActSt(() => classRefs.actSts.dodge.Enter(), cpId);
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
