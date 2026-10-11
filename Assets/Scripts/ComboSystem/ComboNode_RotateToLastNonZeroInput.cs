using System;

public class ComboNode_RotateToLastNonZeroInput : IComboNode_CpHumanoid, IComboNodeTransitionsHolder {
    float animDur;

    public AnimInfo AnimInfo { get; set; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_RotateToLastNonZeroInput(float animDur,CpHumanoidAnimInfoT animInfoT) {
        this.animDur = animDur;
        AnimInfo = CpHumdAnimInfoFactory.Construct(animInfoT);
    }

    public Func<IFsmSt_Cp> GetEnterFunc(CpHumdHandle cp) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_RotateToLastNonZeroInput thisNode = this;
        return () => cp.HumdData.classRefs.actSts
            .comboBranch_RotateToLastNonZeroInputDir.Enter(thisNode, animDur);
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
