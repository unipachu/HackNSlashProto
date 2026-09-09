using System;
using Unity.Mathematics;

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
    /// NOTE: Use this when transitioning from neutral states like walk or idle. For combo chain transitions,
    /// use: <see cref="TryComboTransition"/>. (6.9.2026)
    /// </summary>
    public static Func<IFsmSt_Cp> FindStateEnterFunc(BufferableInput input, int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var unityComps = CpMgr.inst.unityComps[cpId];
        if(input == BufferableInput.BtnE)
            return () => classRefs.actSts.dodge.Enter();
        if (unityComps.rHandItem is IHandItem_Comboer comboer) {
            Func<IFsmSt_Cp> enter = input switch {
                BufferableInput.RShldr => GetEnterFunc(comboer.RShldrComboStart, cpId),
                BufferableInput.RTrg => GetEnterFunc(comboer.RTrgComboStart, cpId),
                BufferableInput.LShldr => GetEnterFunc(comboer.LShldrComboStart, cpId),
                _ => GeneralUtils.LogErrorForInput<BufferableInput, Func<IFsmSt_Cp>>(input)
            };
            if (enter != null)
                return enter;
        }
        // TODO: You should make these "combo" moves, this is just a temp solution.
        if (unityComps.rHandItem is IHandItem_Hitter hitter) {
            if(input == BufferableInput.LShldr)
                return () => classRefs.actSts.atk_FlyingAtk.Enter(
                    new HitEffects(1, KnockbackT.Weak, 5),
                    hitter.HitDealer
                );
            if(input == BufferableInput.RTrg)
                return () => classRefs.actSts.atk_Jump.Enter(
                    new HitEffects(1, KnockbackT.Weak, 1),
                    hitter.HitDealer
                );
        }

        return null;
        // Helper
        static Func<IFsmSt_Cp> GetEnterFunc(IComboNode comboStart, int cpId)
            => comboStart == null ? null : comboStart.GetEnterFunc(cpId);
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
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cpId);
        else
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cpId);
    }

    /// <summary>
    /// Tries to switch state based on input and equipped items (and possibly other state).
    /// Returns true if state transition successful.
    /// </summary>
    public static bool TrySwitchSt(int cpId, BufferableInput input) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var soaData = CpMgr.inst.soaData;
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        Func<IFsmSt_Cp> enterFunc = FindStateEnterFunc(input, cpId);
        if (
            enterFunc != null
                && CpInputBuffer.TryConsumeInput(
                    cpId,
                    input,
                    soaData.inputBuffer_BufferedInput,
                    soaData.inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.SwitchActSt(enterFunc, cpId);
            return true;
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
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.falling.Enter(), cpId);
            return true;
        }
        return false;
    }

    public static void UpdateMovData(
        int id,
        Cp_SoaData data,
        in float2 tgtHorDir,
        in float3 additionalLinMov,
        float tgtHorSpd,
        float yawSpd,
        float horAcc
    ) {
        data.movInput_tgtHorDir[id] = tgtHorDir;
        data.movInput_additionalLinMov[id] = additionalLinMov;
        data.movInput_tgtHorSpd[id] = tgtHorSpd;
        data.movInput_yawSpd[id] = yawSpd;
        data.movInput_horAcc[id] = horAcc;
    }

    /// <summary>
    /// Transitions to any existing next combo node that require input if such input was buffered.
    /// Immediately returns true if successfully switched state.
    /// </summary>
    public static bool TryAnyComboInputTransition(int cpId, IComboNode curComboNode)
    => TryComboTransition(BufferableInput.RShldr, curComboNode, cpId)
        || TryComboTransition(BufferableInput.RTrg, curComboNode, cpId)
        || TryComboTransition(BufferableInput.BtnE, curComboNode, cpId)
        || TryComboTransition(BufferableInput.LShldr, curComboNode, cpId);

    /// <summary>
    /// Returns true if successfully transitioned to the next action state of the combo.
    /// </summary>
    static bool TryComboTransition(BufferableInput input, IComboNode curComboNode, int cpId) {
        var data = CpMgr.inst.soaData;
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        if (
            // TODO: This check if faster than trying to get the next node func. However for simplicity you
            // TODO C: could just consume the input, get the func and then check if it's null. You only gain
            // TODO C: perf only when the button actually doesn't change the state which is cheap anyway.
            curComboNode.GetNextNode(input) != null
                && CpInputBuffer.TryConsumeInput(
                    cpId,
                    input,
                    data.inputBuffer_BufferedInput,
                    data.inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.SwitchActSt(curComboNode.GetNextNode(input).GetEnterFunc(cpId), cpId);
            return true;
        }
        return false;
    }
}
