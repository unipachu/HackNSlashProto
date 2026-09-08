using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    int cpId;

    public CpSt_Dodge(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        Cp_SoaData data = CpMgr.inst.soaData;
        // TODO MINOR: Rename to more generic yawinputrotallowed
        data.actStSt_ImpactInputRotAllowed[cpId] = false;
        data.actStSt_BufferedInputStSwitchAllowed[cpId] = false;
        data.invul[cpId] = true;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.dodge),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.inst.soaData.invul[cpId] = false;
    }

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        float angSpd = 0;
        if (data.actStSt_ImpactInputRotAllowed[cpId])
            angSpd = data.st_Dodge_YawSpd[cpId];
        CpUtils.UpdateMovData(
            cpId,
            data,
            data.input_mov[cpId],
            data.animDPos[cpId],
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (data.actStSt_BufferedInputStSwitchAllowed[cpId] && CpUtils.BaseTrySwitchStByBufferedInput(cpId))
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
