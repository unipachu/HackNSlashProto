using System;
using UnityEngine;

public class ComboNode_ShootProj : IComboNode_CpHumanoid, IComboNodeTransitionsHolder {
    IHandItem_ProjectileSpawner projSpawner;

    public AnimInfo AnimInfo { get; }
    public ComboNode_Transitions Transitions { get; set; }

    public ComboNode_ShootProj(CpHumanoidAnimInfoT animInfoT, UnityEngine.Object ctx) {
        AnimInfo = CpAnimInfoFactory.Construct(animInfoT);
        IHandItem_ProjectileSpawner projSpawner = (IHandItem_ProjectileSpawner)ctx;
        Debug.Assert(
            projSpawner != null,
            $"{ctx.name} didn't implement {nameof(IHandItem_ProjectileSpawner)}",
            ctx
        );
        this.projSpawner = projSpawner;
    }

    public Func<IFsmSt_Cp> GetEnterFunc(CpHandle cp) {
        ComboNode_ShootProj thisNode = this;
        return () => cp.HumdData.classRefs.actSts.comboBranch_ShootHomingProj.Enter(
            thisNode,
            thisNode.projSpawner.ProjHitEffects,
            thisNode.projSpawner.HomingProjData,
            thisNode.projSpawner.ProjSpawnPoseTrf,
            thisNode.projSpawner.ProjT,
            cp.CommonData.classRefs.lockOnTgt.LockOnTrf
        );
    }

    public IComboNode_CpHumanoid GetNextNode(BufferableInput input) {
        return input switch {
            BufferableInput.None => Transitions.node_NoInput,
            BufferableInput.RShldr => Transitions.node_RShldr,
            BufferableInput.RTrg => Transitions.node_RTrg,
            BufferableInput.LShldr => Transitions.node_LShldr,
            BufferableInput.BtnE => Transitions.node_BtnE,
            _ => GeneralUtils.LogErrorForInput<BufferableInput, IComboNode_CpHumanoid>(input)
        };
    }
}
