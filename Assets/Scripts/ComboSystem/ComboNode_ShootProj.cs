using System;
using UnityEngine;

public class ComboNode_ShootProj : IComboNode, IComboNodeTransitionsHolder {
    IHandItem_Hitter hitter;
    IHandItem_ProjectileSpawner projSpawner;

    public AnimInfo AnimInfo { get; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_ShootProj(UnityEngine.Object handItemCtx, CpAnimInfoT animInfoT) {
        AnimInfo = CpAnimInfoFactory.Construct(animInfoT);
        IHandItem_Hitter handItem_HitDealer = (IHandItem_Hitter)handItemCtx;
        Debug.Assert(
            handItem_HitDealer != null,
            $"{handItemCtx.name} didn't implement {nameof(IHandItem_Hitter)}",
            handItemCtx
        );
        this.hitter = handItem_HitDealer;
        IHandItem_ProjectileSpawner projSpawner = (IHandItem_ProjectileSpawner)handItemCtx;
        Debug.Assert(
            projSpawner != null,
            $"{handItemCtx.name} didn't implement {nameof(IHandItem_ProjectileSpawner)}",
            handItemCtx
        );
        this.projSpawner = projSpawner;
    }

    public Func<IFsmSt_Cp> GetEnterFunc(int cpId) {
        ComboNode_ShootProj thisNode = this;
        return () => CpMgr.inst.classRefs[cpId].actSts.atk_ShootHomingProj.Enter(
            thisNode,
            thisNode.hitter.HitEffects,
            thisNode.projSpawner.HomingProjData,
            thisNode.projSpawner.ProjSpawnPoseTrf,
            CpMgr.inst.classRefs[cpId].lockedOnTgt.LockOnTrf
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
