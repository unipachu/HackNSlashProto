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
        CpMgr.GetAos(cp.Id).act_Falling_StartHgt = CpMgr.inst.cp[cp.Id].transform.position.y;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.falling),
            4 // NOTE: Transition is long to give a sense of accleration during falling.
        );
        return this;
    }
    
    public void Tick() {
        var unityComps = CpMgr.inst.unityComps[cp.Id];
        var classRefs = CpMgr.inst.classRefs[cp.Id];
        CpUtils.UpdateMovInputData(
            cp.Id,
            float2.zero,
            float3.zero,
            CpMgr.GetAos(cp.Id).act_Falling_TgtHorSpd,
            0,
            CpMgr.GetAos(cp.Id).act_Falling_HorAcc
        );
        if (CpMgr.GetAos(cp.Id).isGrounded){
            float fallDist = CpMgr.GetAos(cp.Id).act_Falling_StartHgt - CpMgr.inst.cp[cp.Id].transform.position.y;
            if(fallDist > CpMgr.GetAos(cp.Id).act_Falling_LandingStFallDistThreshold) {
                CpMgr.inst.SwitchActSt(() => classRefs.actSts.fallLanding.Enter(), cp.Id);
                return;
            }
            CpUtils.TransitionToFallIdleOrWalk(cp.Id);
            return;
        }
        if (CpMgr.GetAos(cp.Id).curStDur > 15) {
            Debug.LogError($"{cp.Id} likely stuck falling as curStDur was: {CpMgr.GetAos(cp.Id).curStDur}.");
        }
    }
}
