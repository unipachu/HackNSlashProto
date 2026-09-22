using Unity.Mathematics;
using UnityEngine;

public class CpSt_Death : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Death(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => false; // Cannot change to anything when dying.

    public CpSt_Death Enter(AnimInfo deathAnim) {
        cp.Data.ignoreHits = true;
        cp.Data.action_died?.Invoke(cp);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityComps.anim,
            deathAnim,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpMgr.inst.MarkForPendingUnregister(cp.I);
                return;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                return;
        }
    }

    public void Tick() {
        CpUtils.UpdateMovInputData(
            cp.I,
            float2.zero,
            cp.Data.animDPos,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
