using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Dodge(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        CpMgr.GetAos(cp.Id).inputRotAllowed = false;
        CpMgr.GetAos(cp.Id).bufferedInputStSwitchAllowed = false;
        CpMgr.GetAos(cp.Id).invul = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetAos(cp.Id).invul = false;
    }

    public void Tick() {
        float angSpd = 0;
        if (CpMgr.GetAos(cp.Id).inputRotAllowed)
            angSpd = CpMgr.GetAos(cp.Id).act_Dodge_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.Id,
            CpMgr.GetAos(cp.Id).input_mov,
            CpMgr.GetAos(cp.Id).animDPos * CpMgr.GetAos(cp.Id).act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            CpMgr.GetAos(cp.Id).bufferedInputStSwitchAllowed
                && CpUtils.TrySwitchStByBufferedInput(cp.Id)
        )
            return;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.unityComps[cp.Id];
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
