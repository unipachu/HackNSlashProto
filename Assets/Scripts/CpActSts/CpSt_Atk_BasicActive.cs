using UnityEngine;

// TODO: Rename to BasicImpact.
public class CpSt_Atk_BasicActive : IFsmSt_Cp {
    int cpId;
    HitDealer hitDealer;
    IComboNode comboNode;

    public CpSt_Atk_BasicActive(int cpId) {
        this.cpId = cpId;
    }

    public CpSt_Atk_BasicActive Enter(
        // TODO: Weapon stats should affect attack. Implement them to the combo node at some point.
        //int baseDmg,
        IComboNode comboNode,
        HitDealer hitDealer
    ) {
        this.comboNode = comboNode;
        this.hitDealer = hitDealer;
        var data = CpMgr.inst.soaData;
        var unityComps = CpMgr.inst.unityComps;
        data.actStSt_AtkPhase[cpId] = AtkPhase.Impact;
        data.actStSt_ComboAllowed[cpId] = false;
        data.actStSt_InputRotAllowed[cpId] = false;
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
                // TODO: Item
                hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                hitDealer.hitWldDir = unityComps.trf.forward;
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
        var data = CpMgr.inst.soaData;
        var classRefs = CpMgr.inst.unityComps[cpId];
        float angSpd = 0;
        if (data.actStSt_InputRotAllowed[cpId])
            // TODO: Change the name of this to generic yaw speed.
            angSpd = data.st_AtkHorSlash_Impact_AngSpd[cpId];
        CpUtils.UpdateMovData(
            cpId,
            data,
            data.input_mov[cpId],
            data.animDPos[cpId],
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.RShldr, comboNode, cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.RTrg, comboNode, cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.BtnE, comboNode, cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.LShldr, comboNode, cpId))
            return;
    }
}
