using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    int cpId;

    public CpSt_Dodge(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        CpMgr.GetSoa(cpId).actStSt_InputRotAllowed = false;
        CpMgr.GetSoa(cpId).actStSt_BufferedInputStSwitchAllowed = false;
        CpMgr.GetSoa(cpId).invul = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetSoa(cpId).invul = false;
    }

    public void Tick() {
        Cp_AosData aosData = CpMgr.inst.aosData[cpId];
        float angSpd = 0;
        if (CpMgr.GetSoa(cpId).actStSt_InputRotAllowed)
            angSpd = CpMgr.GetSoa(cpId).st_Dodge_YawSpd;
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetSoa(cpId).input_mov,
            CpMgr.GetSoa(cpId).animDPos * aosData.dodgeHorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            CpMgr.GetSoa(cpId).actStSt_BufferedInputStSwitchAllowed
                && CpUtils.BaseTrySwitchStByBufferedInput(cpId)
        )
            return;
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.unityComps[cpId];
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
