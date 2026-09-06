using System;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;

/// <summary>
/// Capsule pawn general util methods. Consider organizing these better!
/// </summary>
public static class CpUtils{
    /// <summary>
    /// Can be used from neutral states like "walk" or "idle" to transition to new states with input.<br/>
    /// Returns true if succeeded changing state.
    /// </summary>
    // TODO: Rename?
    public static bool BaseTrySwitchStByBufferedInput(int cpId) {
        if (TrySwitchSt(cpId, BufferableInput.BtnE))
            return true;
        if (TrySwitchSt(cpId, BufferableInput.RShldr))
            return true;
        if (TrySwitchSt(cpId, BufferableInput.RTrg))
            return true;
        if (TrySwitchSt(cpId, BufferableInput.LShldr))
            return true;
        return false;
    }

    /// <summary>
    /// Finds next state to transition to based on input and held items. Returns null if no applicable
    /// state found.<br/>
    /// NOTE: For combo chain transitions, use: <see cref="TryComboTransition"/>. (6.9.2026)
    /// </summary>
    public static Func<IFsmSt_Cp> FindState(BufferableInput input, int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var unityComps = CpMgr.inst.unityComps[cpId];
        if(input == BufferableInput.BtnE)
            return () => classRefs.actSts.dodge.Enter();
        if (unityComps.rHandItem is IComboUser) {
            IComboUser meleeHitDealer = (IComboUser)CpMgr.inst.unityComps[cpId].rHandItem;
            switch (input) {
                case BufferableInput.RShldr:
                    return meleeHitDealer.RShldrComboStart.GetEnterFunc(cpId);
                case BufferableInput.RTrg:
                    return meleeHitDealer.RTrgComboStart.GetEnterFunc(cpId);
                case BufferableInput.LShldr:
                    return meleeHitDealer.LShldrComboStart.GetEnterFunc(cpId);
                default:
                    break;
            }
        }
        return null;
    }

    /// <summary>
    /// Used to transition to a baic action state after an attack/special move etc.
    /// </summary>
    // TODO: rename?
    public static void TransitionToFallIdleOrWalk(int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        SwitchToFallingStIfNotGrounded(cpId);
        if (math.all(CpMgr.inst.soaData.input_mov[cpId] != float2.zero))
            CpMgr.inst.SwitchToActSt(() => classRefs.actSts.walk.Enter(), cpId);
        else
            CpMgr.inst.SwitchToActSt(() => classRefs.actSts.idle.Enter(), cpId);
    }

    /// <summary>
    /// Tries to switch state based on input and equipped items (and possibly other state).
    /// Returns true if state transition successful.
    /// </summary>
    public static bool TrySwitchSt(int cpId, BufferableInput input) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var soaData = CpMgr.inst.soaData;
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        if (
            CpInputBuffer.TryConsumeInput(
                cpId,
                input,
                soaData.inputBuffer_BufferedInput,
                soaData.inputBuffer_RemainingTime
            )
        ) {
            var enterFunc = FindState(input, cpId);
            if (enterFunc != null) {
                CpMgr.inst.SwitchToActSt(enterFunc, cpId);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        if (
            !CpMgr.inst.soaData.isGrounded[cpId]
            && classRefs.st_cur.GetType() != typeof(CpSt_Falling)
        ) {
            //Debug.Log($"{id} was not grounded so switch to falling st!");
            CpMgr.inst.SwitchToActSt(() => classRefs.actSts.falling.Enter(), cpId);
            return true;
        }
        return false;
    }

    public static void UpdateMovData(
        int id,
        Cp_SoaData data,
        in float2 horMov,
        in float3 animRootMov,
        float maxLinSpd,
        float yawSpd,
        float linAcc
    ) {
        data.mov_horMov[id] = horMov;
        data.mov_animRootMot[id] = animRootMov;
        data.mov_maxLinSpd[id] = maxLinSpd;
        data.mov_yawSpd[id] = yawSpd;
        data.mov_linAcc[id] = linAcc;
    }

    /// <summary>
    /// Returns true if successfully transitioned to the next action state of the combo.
    /// </summary>
    public static bool TryComboTransition(BufferableInput input, IComboNode comboNode, int cpId) {
        var data = CpMgr.inst.soaData;
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        if (
            // TODO: This check if faster than trying to get the next node func. However for simplicity you
            // TODO C: could just consume the input, get the func and then check if it's null. You only gain
            // TODO C: perf only when the button actually doesn't change the state which is cheap anyway.
            comboNode.NextNodeI(input) != -1
                && CpInputBuffer.TryConsumeInput(
                    cpId,
                    input,
                    data.inputBuffer_BufferedInput,
                    data.inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.SwitchToActSt(comboNode.GetNextNode(input).GetEnterFunc(cpId), cpId);
            return true;
        }
        return false;
    }
}
