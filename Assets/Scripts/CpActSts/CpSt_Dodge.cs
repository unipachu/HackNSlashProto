using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Dodge(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        CpMgr.GetData(cp.Id).inputRotAllowed = false;
        CpMgr.GetData(cp.Id).bufferedInputStSwitchAllowed = false;
        CpMgr.GetData(cp.Id).invul = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetData(cp.Id).invul = false;
    }

    public void Tick() {
        float angSpd = 0;
        if (CpMgr.GetData(cp.Id).inputRotAllowed)
            angSpd = CpMgr.GetData(cp.Id).act_Dodge_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.Id,
            CpMgr.GetData(cp.Id).input_mov,
            CpMgr.GetData(cp.Id).animDPos * CpMgr.GetData(cp.Id).act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            CpMgr.GetData(cp.Id).bufferedInputStSwitchAllowed
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
