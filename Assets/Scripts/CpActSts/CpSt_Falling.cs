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
        cp.Data.act_Falling_StartHgt = cp.Data.handle.transform.position.y;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityComps.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.falling),
            4 // NOTE: Transition is long to give a sense of accleration during falling. // TODO: So.
        );
        return this;
    }
    
    public void Tick() {
        var unityComps = CpMgr.inst.aos[cp.I].unityComps;
        var classRefs = cp.Data.classRefs;
        CpUtils.UpdateMovInputData(
            cp.I,
            float2.zero,
            float3.zero,
            cp.Data.act_Falling_TgtHorSpd,
            0,
            cp.Data.act_Falling_HorAcc
        );
        if (cp.Data.isGrounded){
            //Debug.Log("Is grounded");
            float fallDist = cp.Data.act_Falling_StartHgt - cp.Data.handle.transform.position.y;
            if(fallDist > cp.Data.act_Falling_LandingStFallDistThreshold) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.fallLanding.Enter(), cp.I);
                return;
            }
            CpUtils.TransitionToFallIdleOrWalk(cp.I);
            return;
        }
        Debug.Assert(
            cp.Data.curStDur <= 15, $"{cp.I} likely stuck falling as curStDur was: {cp.Data.curStDur}.", cp);
    }
}
