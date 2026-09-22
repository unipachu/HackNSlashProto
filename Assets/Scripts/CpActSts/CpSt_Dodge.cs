using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Dodge(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        CpMgr.GetData(cp.I).inputRotAllowed = false;
        CpMgr.GetData(cp.I).bufferedInputStSwitchAllowed = false;
        CpMgr.GetData(cp.I).ignoreHits = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetData(cp.I).ignoreHits = false;
    }

    public void Tick() {
        float angSpd = 0;
        if (CpMgr.GetData(cp.I).inputRotAllowed)
            angSpd = CpMgr.GetData(cp.I).act_Dodge_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.I,
            CpMgr.GetData(cp.I).input_mov,
            CpMgr.GetData(cp.I).animDPos * CpMgr.GetData(cp.I).act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            CpMgr.GetData(cp.I).bufferedInputStSwitchAllowed
                && CpUtils.TrySwitchStByBufferedInput(cp.I)
        )
            return;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.unityComps[cp.I];
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
