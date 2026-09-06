using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// TODO: Rename to CpSt...
/// <summary>
/// Action which always leads to one of the basic action (walk, idle, fall).
/// NOTE: Recovery action cannot be interrupted by input! If that's what you want, then use some other
/// NOTE C: action e.g. BasicImpact. (6.9.2026)
/// </summary>
public class Cp_Atk_BasicRecovery : IFsmSt_Cp {
    int cpId;

    public Cp_Atk_BasicRecovery(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public void Exit() {
    }

    public Cp_Atk_BasicRecovery Enter(AnimInfo animInfo) {
        var data = CpMgr.inst.soaData;
        var unityComps = CpMgr.inst.unityComps;
        data.actStSt_AtkPhase[cpId] = AtkPhase.Recovery;
        data.actStSt_ComboAllowed[cpId] = false;
        data.actStSt_ImpactInputRotAllowed[cpId] = false;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            unityComps[cpId].anim,
            animInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cpId);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        // interpolate to walking speed.
        data.actStSt_RecoveryMotInterpTimer[cpId] += Time.deltaTime;
        // TODO: Make interp value SO field.
        float interpValue = Mathf.Clamp01(data.actStSt_RecoveryMotInterpTimer[cpId] / 0.2f);
        CpUtils.UpdateMovData(
            cpId,
            data,
            data.input_mov[cpId],
            float3.zero,
            data.st_Walk_MaxLinSpd[cpId] * interpValue,
            data.st_Walk_YawSpd[cpId] * interpValue,
            data.st_Walk_LinAcc[cpId]
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        if (data.actStSt_DodgeAllowed[cpId]) {
            if (CpInputBuffer.TryConsumeInput(
                cpId,
                BufferableInput.BtnE,
                data.inputBuffer_BufferedInput,
                data.inputBuffer_RemainingTime)
            ) {
                CpMgr.inst.SwitchToActSt(() => classRefs.actSts.dodge.Enter(),cpId);
                return;
            }
        }
    }
}
