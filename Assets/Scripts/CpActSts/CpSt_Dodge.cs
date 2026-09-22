using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Dodge(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        cp.Data.inputRotAllowed = false;
        cp.Data.bufferedInputStSwitchAllowed = false;
        cp.Data.ignoreHits = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityComps.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        cp.Data.ignoreHits = false;
    }

    public void Tick() {
        float angSpd = 0;
        if (cp.Data.inputRotAllowed)
            angSpd = cp.Data.act_Dodge_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov,
            cp.Data.animDPos * cp.Data.act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            cp.Data.bufferedInputStSwitchAllowed
                && CpUtils.TrySwitchStByBufferedInput(cp.I)
        )
            return;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.aos[cp.I].unityComps;
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
