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
        data.actStSt_InputRotAllowed[cpId] = false;
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
        Cp_SoaData soaData = CpMgr.inst.soaData;
        Cp_AosData aosData = CpMgr.inst.aosData[cpId];
        float angSpd = 0;
        if (soaData.actStSt_InputRotAllowed[cpId])
            angSpd = soaData.st_Dodge_YawSpd[cpId];
        CpUtils.UpdateMovInputData(
            cpId,
            soaData,
            soaData.input_mov[cpId],
            soaData.animDPos[cpId] * aosData.dodgeHorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (soaData.actStSt_BufferedInputStSwitchAllowed[cpId] && CpUtils.BaseTrySwitchStByBufferedInput(cpId))
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
