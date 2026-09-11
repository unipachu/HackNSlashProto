using Unity.Mathematics;
using UnityEngine;

public class CpSt_Falling : IFsmSt_Cp {
    int cpId;

    public CpSt_Falling(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Falling Enter() {
        CpMgr.GetAos(cpId).act_Falling_StartHgt = CpMgr.GetAos(cpId).trf_pos.y;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.falling),
            4 // NOTE: Transition is long to give a sense of accleration during falling.
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {}
    
    public void LateTick() {}

    public void PhysicsTick() {}
    
    public void Tick() {
        var unityComps = CpMgr.inst.unityComps[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        CpUtils.UpdateMovInputData(
            cpId,
            float2.zero,
            float3.zero,
            CpMgr.GetAos(cpId).act_Falling_TgtHorSpd,
            0,
            CpMgr.GetAos(cpId).act_Falling_HorAcc
        );
        if (CpMgr.GetAos(cpId).isGrounded){
            float fallDist = CpMgr.GetAos(cpId).act_Falling_StartHgt - CpMgr.GetAos(cpId).trf_pos.y;
            if(fallDist > CpMgr.GetAos(cpId).act_Falling_LandingStFallDistThreshold) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.fallLanding.Enter(), cpId);
                return;
            }
            CpUtils.TransitionToFallIdleOrWalk(cpId);
            return;
        }
        if (CpMgr.GetAos(cpId).curStDur > 15) {
            Debug.LogError($"{cpId} likely stuck falling as curStDur was: {CpMgr.GetAos(cpId).curStDur}.");
        }
    }
}
