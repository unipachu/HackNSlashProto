using System;

/// <summary>
/// Recovery move - ends a combo chain and cannot be input canceled to other moves.<br/>
/// </summary>
public class ComboNode_BasicRecovery : IComboNode_CpHumanoid {
    public AnimInfo AnimInfo { get; }

    public ComboNode_BasicRecovery(CpHumanoidAnimInfoT animInfoT) {
        AnimInfo = CpAnimInfoFactory.Construct(animInfoT);
    }

    public Func<IFsmSt_Cp> GetEnterFunc(CpHandle cp) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_BasicRecovery thisNode = this;
        return () => cp.HumdData.classRefs.actSts.comboEnd_BasicRecovery.Enter(thisNode.AnimInfo);
    }

    public IComboNode_CpHumanoid GetNextNode(BufferableInput input)
        => null;
}