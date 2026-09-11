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
        Cp_SoaData data = CpMgr.inst.soaData;
        data.actStSt_FallingStartHgt[cpId] = data.trf_pos[cpId].y;
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
        Cp_SoaData soaData = CpMgr.inst.soaData;
        var unityComps = CpMgr.inst.unityComps[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        CpUtils.UpdateMovInputData(
            cpId,
            soaData,
            float2.zero,
            float3.zero,
            soaData.st_Falling_TgtHorSpd[cpId],
            0,
            soaData.st_Falling_HorAcc[cpId]
        );
        if (soaData.isGrounded[cpId]){
            float fallDist = soaData.actStSt_FallingStartHgt[cpId] - soaData.trf_pos[cpId].y;
            if(fallDist > soaData.st_Falling_LandingStFallDistThreshold[cpId]) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.fallLanding.Enter(), cpId);
                return;
            }
            CpUtils.TransitionToFallIdleOrWalk(cpId);
            return;
        }
        if (soaData.curStDur[cpId] > 15) {
            Debug.LogError($"{cpId} likely stuck falling as curStDur was: {soaData.curStDur[cpId]}.");
        }
    }
}
