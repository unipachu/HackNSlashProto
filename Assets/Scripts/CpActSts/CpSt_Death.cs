using Unity.Mathematics;
using UnityEngine;

public class CpSt_Death : IFsmSt_Cp {
    int cpId;

    public CpSt_Death(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => false; // Cannot change to anything when dying.

    public CpSt_Death Enter(AnimInfo deathAnim) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            deathAnim,
            0.1f
        );
        return this;
    }

    public void Exit() { }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                GameObject.Destroy(CpMgr.inst.unityComps[cpId].rootTrf.gameObject);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() { }

    public void PhysicsTick() { }

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        CpUtils.UpdateMovInputData(
            cpId,
            data,
            float2.zero,
            data.animDPos[cpId],
            0,
            0,
            float.PositiveInfinity
        );
    }
}
