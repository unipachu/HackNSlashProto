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
        CpMgr.GetAos(cp.Id).act_AtkPhase = AtkPhase.Impact;
        CpMgr.GetAos(cp.Id).comboAllowed = false;
        CpMgr.GetAos(cp.Id).inputRotAllowed = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            unityComps[cp.Id].anim,
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
        var classRefs = CpMgr.inst.classRefs[cp.Id];
        var unityComps = CpMgr.inst.unityComps[cp.Id];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.Id),
                        cp.Id
                    );
                    return;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                //Debug.Log($"rHandEquippable null: {classRefs.rHandEquippable == null}");
                hitDealer.hitData = new HitData(hitEffects, CpMgr.GetAos(cp.Id).team, cp.transform.forward);
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
        var classRefs = CpMgr.inst.unityComps[cp.Id];
        float angSpd = 0;
        if (CpMgr.GetAos(cp.Id).inputRotAllowed)
            angSpd = CpMgr.GetAos(cp.Id).act_BasicImpact_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.Id,
            CpMgr.GetAos(cp.Id).input_mov,
            CpMgr.GetAos(cp.Id).animDPos,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.Id))
            return;
        if (CpMgr.GetAos(cp.Id).comboAllowed && CpUtils.TryAnyComboInputTransition(cp.Id, comboNode))
            return;
    }
}
