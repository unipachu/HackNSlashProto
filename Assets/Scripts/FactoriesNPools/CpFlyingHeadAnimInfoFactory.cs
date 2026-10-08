using UnityEngine;

public static class CpFlyingHeadAnimInfoFactory{
    public static AnimInfo Construct(CpFlyingHeadAnimInfoT t) {
        return t switch {
            CpFlyingHeadAnimInfoT.atk_DashAtk_Impact => throw new System.NotImplementedException(),
            CpFlyingHeadAnimInfoT.atk_DashAtk_Recovery => throw new System.NotImplementedException(),
            CpFlyingHeadAnimInfoT.atk_DashAtk_Windup => throw new System.NotImplementedException(),
            CpFlyingHeadAnimInfoT.idle => new(
                Animator.StringToHash("CpFlyingHeadVis_Idle"), 0, true, 50),
            CpFlyingHeadAnimInfoT.knockback_Weak_Bwd => throw new System.NotImplementedException(),
            CpFlyingHeadAnimInfoT.knockback_Weak_Fwd => throw new System.NotImplementedException(),
            _ => GeneralUtils.LogErrorForInput<CpFlyingHeadAnimInfoT, AnimInfo>(t)
        };
    }
}
