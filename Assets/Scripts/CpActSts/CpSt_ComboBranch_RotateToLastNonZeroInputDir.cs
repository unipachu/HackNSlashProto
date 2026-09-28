using UnityEngine;

/// <summary>
/// NOTE: The name of this is not very descriptive. This will use last non zero movement input to yaw the
/// character but target hor movement is 0. So input still works for rotation.
/// </summary>
public class CpSt_ComboBranch_RotateToLastNonZeroInputDir : IFsmSt_Cp {
    CpHandle cp;
    IComboNode comboNode;

    public CpSt_ComboBranch_RotateToLastNonZeroInputDir(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_RotateToLastNonZeroInputDir Enter(IComboNode comboNode) {
        this.comboNode = comboNode;
        InputBufferUtils.Clear(
            ref cp.Data.inputBuffer_BufferedInput,
            ref cp.Data.inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.Data.unityObjs.anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = cp.Data.classRefs;
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
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        ref var cpData = ref cp.Data;
        //Dbg.Log(
        //    $"Cp {cp.I}: {nameof(cpData.act_BasicWindup_MaxAngSpd)}: {cpData.act_BasicWindup_MaxAngSpd}",
        //    cpData.enableDbgMsgs
        //);
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov_LastNonZero,
            cpData.animDPos,
            0,
            cpData.act_BasicWindup_MaxAngSpd, // TODO: tgtHorSpd, yawSpd, and horAcc should be set in the state Enter method, since these are currently are set to work with melee windup moves and nothing else.
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cpData.comboAllowed && CpUtils.TryAnyComboInputTransition(cp.I, comboNode))
            return;
    }
}
