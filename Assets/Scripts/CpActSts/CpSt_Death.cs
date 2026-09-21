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
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            deathAnim,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpMgr.inst.UnregisterNDestroy(cp.Id);
                return;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                return;
        }
    }

    public void Tick() {
        CpUtils.UpdateMovInputData(
            cp.Id,
            float2.zero,
            CpMgr.GetAos(cp.Id).animDPos,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
