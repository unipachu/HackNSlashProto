using System;

/// <summary>
/// Recovery move - ends a combo chain and cannot be input canceled to other moves.<br/>
/// </summary>
public class ComboNode_BasicRecovery : IComboNode {
    public AnimInfo AnimInfo { get; }

    public ComboNode_BasicRecovery(CpAnimInfoT animInfoT) {
        AnimInfo = CpAnimInfo.Get(animInfoT);
    }

    public Func<IFsmSt_Cp> GetEnterFunc(int cpId) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_BasicRecovery thisNode = this;
        return () => CpMgr.inst.classRefs[cpId].actSts.atk_BasicRecovery.Enter(thisNode.AnimInfo);
    }

    public IComboNode GetNextNode(BufferableInput input)
        => null;
}