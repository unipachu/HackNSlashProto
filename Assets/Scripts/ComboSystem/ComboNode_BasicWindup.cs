using System;

/// <summary>
/// Windup combo move. Allows input canceling to other moves. <br/>
/// NOTE: Most windup moves will not allow input canceling - set those node indexes to -1 (5.9.2026)
/// </summary>
public class ComboNode_BasicWindup : IComboNode, IComboNodeTransitionsHolder {
    public AnimInfo AnimInfo { get; set; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_BasicWindup(CpAnimInfoT animInfoT) {
        AnimInfo = CpAnimInfo.Get(animInfoT);
    }

    public Func<IFsmSt_Cp> GetEnterFunc(int cpId) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_BasicWindup thisNode = this;
        return () => CpMgr.inst.classRefs[cpId].actSts.atk_BasicWindup.Enter(thisNode);
    }

    public IComboNode GetNextNode(BufferableInput input) {
        return input switch {
            BufferableInput.None => Transitions.node_NoInput,
            BufferableInput.RShldr => Transitions.node_RShldr,
            BufferableInput.RTrg => Transitions.node_RTrg,
            BufferableInput.LShldr => Transitions.node_LShldr,
            BufferableInput.BtnE => Transitions.node_BtnE,
            _ => StructUtils.LogErrorForInput<BufferableInput, IComboNode>(input)
        };
    }
}