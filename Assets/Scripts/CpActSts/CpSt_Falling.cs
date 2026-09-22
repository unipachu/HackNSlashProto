using Unity.Mathematics;
using UnityEngine;

public class CpSt_Falling : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Falling(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Falling Enter() {
        cp.Data.act_Falling_StartHgt = CpMgr.inst.handle[cp.I].transform.position.y;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.falling),
            4 // NOTE: Transition is long to give a sense of accleration during falling.
        );
        return this;
    }
    
    public void Tick() {
        var unityComps = CpMgr.inst.unityComps[cp.I];
        var classRefs = CpMgr.inst.classRefs[cp.I];
        CpUtils.UpdateMovInputData(
            cp.I,
            float2.zero,
            float3.zero,
            cp.Data.act_Falling_TgtHorSpd,
            0,
            cp.Data.act_Falling_HorAcc
        );
        if (cp.Data.isGrounded){
            float fallDist = cp.Data.act_Falling_StartHgt - CpMgr.inst.handle[cp.I].transform.position.y;
            if(fallDist > cp.Data.act_Falling_LandingStFallDistThreshold) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.fallLanding.Enter(), cp.I);
                return;
            }
            CpUtils.TransitionToFallIdleOrWalk(cp.I);
            return;
        }
        if (cp.Data.curStDur > 15) {
            Debug.LogError($"{cp.I} likely stuck falling as curStDur was: {cp.Data.curStDur}.");
        }
    }
}
