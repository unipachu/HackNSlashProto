using System;

/// <summary>
/// Branching combo move (can be used for e.g. windups and simple transitions). Allows input canceling to
/// other moves.<br/>
/// </summary>
public class ComboNode_RotateToWhenLastSwitchedStInputDir : IComboNode_CpHumanoid, IComboNodeTransitionsHolder {
    public AnimInfo AnimInfo { get; set; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_RotateToWhenLastSwitchedStInputDir(CpHumanoidAnimInfoT animInfoT) {
        AnimInfo = CpHumdAnimInfoFactory.Construct(animInfoT);
    }

    public Func<IFsmSt_Cp> GetEnterFunc(CpHumdHandle cp) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_RotateToWhenLastSwitchedStInputDir thisNode = this;
        return () => cp.HumdData.classRefs.actSts
            .comboBranch_RotateToWhenLastSwitchedStInputDir.Enter(thisNode);
    }

    public IComboNode_CpHumanoid GetNextNode(BufferableInput input) {
        return input switch {
            BufferableInput.None => Transitions.node_NoInput,
            BufferableInput.Rb => Transitions.node_RShldr,
            BufferableInput.Rt => Transitions.node_RTrg,
            BufferableInput.Lb => Transitions.node_LShldr,
            BufferableInput.B => Transitions.node_BtnE,
            _ => GeneralUtils.LogErrorForInput<BufferableInput, IComboNode_CpHumanoid>(input)
        };
    }
}