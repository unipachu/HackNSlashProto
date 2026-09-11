using UnityEngine;

public class CpSt_Atk_BasicImpact : IFsmSt_Cp {
    IComboNode comboNode;
    int cpId;
    HitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_BasicImpact(int cpId) {
        this.cpId = cpId;
    }

    public CpSt_Atk_BasicImpact Enter(HitEffects hitEffects, IComboNode comboNode, HitDealer hitDealer) {
        this.comboNode = comboNode;
        this.hitDealer = hitDealer;
        this.hitEffects = hitEffects;
        var unityComps = CpMgr.inst.unityComps;
        CpMgr.GetSoa(cpId).actStSt_AtkPhase = AtkPhase.Impact;
        CpMgr.GetSoa(cpId).actStSt_ComboAllowed = false;
        CpMgr.GetSoa(cpId).actStSt_InputRotAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            unityComps[cpId].anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public void Exit() {
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var unityComps = CpMgr.inst.unityComps[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpId),
                        cpId
                    );
                    return;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                //Debug.Log($"rHandEquippable null: {classRefs.rHandEquippable == null}");
                hitDealer.hitEffects = hitEffects;
                hitDealer.hitWldDir = unityComps.rootTrf.forward;
                hitDealer.Activate();
                break;
            case CpAnimEventT.HitDealerDeactivated:
                hitDealer.Deactivate();
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        var classRefs = CpMgr.inst.unityComps[cpId];
        float angSpd = 0;
        if (CpMgr.GetSoa(cpId).actStSt_InputRotAllowed)
            angSpd = CpMgr.GetSoa(cpId).actStSt_Impact_YawSpd;
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetSoa(cpId).input_mov,
            CpMgr.GetSoa(cpId).animDPos,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        if (CpMgr.GetSoa(cpId).actStSt_ComboAllowed && CpUtils.TryAnyComboInputTransition(cpId, comboNode))
            return;
    }
}
