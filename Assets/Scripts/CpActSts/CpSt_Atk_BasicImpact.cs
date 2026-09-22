using UnityEngine;

public class CpSt_Atk_BasicImpact : IFsmSt_Cp {
    IComboNode comboNode;
    CpHandle cp;
    HitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_BasicImpact(CpHandle cp) {
        this.cp = cp;
    }

    public CpSt_Atk_BasicImpact Enter(HitEffects hitEffects, IComboNode comboNode, HitDealer hitDealer) {
        this.comboNode = comboNode;
        this.hitDealer = hitDealer;
        this.hitEffects = hitEffects;
        var unityComps = CpMgr.inst.unityComps;
        cp.Data.act_AtkPhase = AtkPhase.Impact;
        cp.Data.comboAllowed = false;
        cp.Data.inputRotAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            unityComps[cp.I].anim,
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
        var classRefs = CpMgr.inst.classRefs[cp.I];
        var unityComps = CpMgr.inst.unityComps[cp.I];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.I),
                        cp.I
                    );
                    return;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                //Debug.Log($"rHandEquippable null: {classRefs.rHandEquippable == null}");
                hitDealer.hitData = new HitData(hitEffects, cp.Data.team, cp.transform.forward);
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

    public void Tick() {
        var classRefs = CpMgr.inst.unityComps[cp.I];
        float angSpd = 0;
        if (cp.Data.inputRotAllowed)
            angSpd = cp.Data.act_BasicImpact_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov,
            cp.Data.animDPos,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cp.Data.comboAllowed && CpUtils.TryAnyComboInputTransition(cp.I, comboNode))
            return;
    }
}
