using Unity.Mathematics;

public class CpSt_Falling : IFsmSt_Cp {
    int cpId;

    public CpSt_Falling(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Falling Enter() {
        Cp_SoaData data = CpMgr.inst.soaData;
        data.actStSt_FallingStartHgt[cpId] = data.trf_pos[cpId].y;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.falling,
            4 // TODO: So?
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {}
    
    public void LateTick() {}

    public void PhysicsTick() {}
    
    public void Tick() {
        Cp_SoaData soaData = CpMgr.inst.soaData;
        var unityComps = CpMgr.inst.unityComps[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        CpUtils.UpdateMovData(
            cpId,
            soaData,
            float2.zero,
            float3.zero,
            // TODO MINOR: You could use st_Falling_MaxLinSpd in here + hor input to
            // TODO MINOR: allow for slight air control.
            0,
            0,
            soaData.st_Falling_LinAcc[cpId]
        );
        if (soaData.isGrounded[cpId]){
            float fallDist = soaData.actStSt_FallingStartHgt[cpId] - soaData.trf_pos[cpId].y;
            // TODO: Make scriptable object field. This decides if the player will go to
            // TODO C: landing animation or straight to idle.
            if(fallDist > 2) {
                CpMgr.inst.SwitchToActSt(() => classRefs.actSts.fallLanding.Enter(), cpId);
                return;
            }
            CpUtils.TransitionToFallIdleOrWalk(cpId);
            return;
        }
        if (soaData.curStDur[cpId] > 20) {
            // TODO: Character stuck falling. Kill/reset character (maybe have a unique
            // TODO C: death state for when character dies like this where the player doesn't
            // TODO C: lose their souls).
        }
    }
}
