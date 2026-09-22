using System;

/// <summary>
/// Branching combo move (can be used for e.g. windups and simple transitions). Allows input canceling to
/// other moves.<br/>
/// </summary>
public class ComboNode_BasicBranch : IComboNode, IComboNodeTransitionsHolder {
    public AnimInfo AnimInfo { get; set; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_BasicBranch(CpAnimInfoT animInfoT) {
        AnimInfo = CpAnimInfoFactory.Construct(animInfoT);
    }

    public Func<IFsmSt_Cp> GetEnterFunc(int cpI) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_BasicBranch thisNode = this;
        return () => CpMgr.inst.aos[cpI].classRefs.actSts.atk_BasicBranch.Enter(thisNode);
    }

    public IComboNode GetNextNode(BufferableInput input) {
        return input switch {
            BufferableInput.None => Transitions.node_NoInput,
            BufferableInput.RShldr => Transitions.node_RShldr,
            BufferableInput.RTrg => Transitions.node_RTrg,
            BufferableInput.LShldr => Transitions.node_LShldr,
            BufferableInput.BtnE => Transitions.node_BtnE,
            _ => GeneralUtils.LogErrorForInput<BufferableInput, IComboNode>(input)
        };
    }
}