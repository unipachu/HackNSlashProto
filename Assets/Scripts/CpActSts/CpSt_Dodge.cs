using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    int cpId;

    public CpSt_Dodge(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        CpMgr.GetAos(cpId).inputRotAllowed = false;
        CpMgr.GetAos(cpId).bufferedInputStSwitchAllowed = false;
        CpMgr.GetAos(cpId).invul = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetAos(cpId).invul = false;
    }

    public void Tick() {
        float angSpd = 0;
        if (CpMgr.GetAos(cpId).inputRotAllowed)
            angSpd = CpMgr.GetAos(cpId).act_Dodge_YawSpd;
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetAos(cpId).input_mov,
            CpMgr.GetAos(cpId).animDPos * CpMgr.GetAos(cpId).act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            CpMgr.GetAos(cpId).bufferedInputStSwitchAllowed
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
