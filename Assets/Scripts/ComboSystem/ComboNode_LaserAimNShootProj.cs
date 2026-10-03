using System;
using UnityEngine;

public class ComboNode_LaserAimNShootProj : IComboNode, IComboNodeTransitionsHolder {
    IHandItem_ProjectileSpawner projSpawner;

    public AnimInfo AnimInfo { get; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_LaserAimNShootProj(CpAnimInfoT animInfoT, UnityEngine.Object ctx) {
        AnimInfo = CpAnimInfoFactory.Construct(animInfoT);
        IHandItem_ProjectileSpawner projSpawner = (IHandItem_ProjectileSpawner)ctx;
        Debug.Assert(
            projSpawner != null,
            $"{ctx.name} didn't implement {nameof(IHandItem_ProjectileSpawner)}",
            ctx
        );
        this.projSpawner = projSpawner;
    }

    public Func<IFsmSt_Cp> GetEnterFunc(int cpI) {
        ComboNode_LaserAimNShootProj thisNode = this;
        return () => CpMgr.inst.aos[cpI].classRefs.actSts.comboBranch_LaserAimNShootHomingProj.Enter(
            thisNode.projSpawner.AimLaser,
            thisNode,
            thisNode.projSpawner.ProjHitEffects,
            thisNode.projSpawner.HomingProjData,
            thisNode.projSpawner.ProjSpawnPoseTrf,
            thisNode.projSpawner.ProjT
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
