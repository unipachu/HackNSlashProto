using System;
using UnityEngine;

/// <summary>
/// Basic impact combo move using a hit dealer (e.g. a melee weapon hit box).<br/>
/// NOTE: Leave node indexes to -1 if you don't want that input to trigger transition. (5.9.2026)
/// </summary>
public class ComboNode_BasicImpact : IComboNode, IComboNodeTransitionsHolder {
    IHandItem_Hitter hitter;

    public AnimInfo AnimInfo { get; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_BasicImpact(UnityEngine.Object ctx, CpAnimInfoT animInfoT) {
        AnimInfo = CpAnimInfo.Get(animInfoT);
        IHandItem_Hitter handItem_HitDealer = (IHandItem_Hitter)ctx;
        Debug.Assert(
            handItem_HitDealer != null,
            $"{ctx.name} didn't implement {nameof(IHandItem_Hitter)}",
            ctx
        );
        hitter = handItem_HitDealer;
    }

    public Func<IFsmSt_Cp> GetEnterFunc(int cpId) {
        // NOTE: For some stupid reason you need to create a local copy of the struct instead of directly
        // NOTE C: passing it to the Enter method. (5.9.2026)
        ComboNode_BasicImpact thisNode = this;
        return () => CpMgr.inst.classRefs[cpId].actSts.atk_BasicImpact.Enter(
            thisNode.hitter.HitEffects,
            thisNode,
            thisNode.hitter.HitDealer
        );
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