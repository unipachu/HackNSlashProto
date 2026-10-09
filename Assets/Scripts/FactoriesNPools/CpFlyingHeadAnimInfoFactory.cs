using UnityEngine;

public static class CpFlyingHeadAnimInfoFactory{
    public static AnimInfo Construct(CpFlyingHeadAnimInfoT t) {
        return t switch {
            CpFlyingHeadAnimInfoT.atk_DashAtk_Impact => new(
                Animator.StringToHash("CpFlyingHeadVis_Atk_Dash_Impact"), 0, false, 22,
                (0, CpAnimEventT.HitDealerActivated),
                (18, CpAnimEventT.HitDealerDeactivated),
                (22, CpAnimEventT.Finished)),
            CpFlyingHeadAnimInfoT.atk_DashAtk_Recovery => new(
                Animator.StringToHash("CpFlyingHeadVis_Atk_Dash_Recovery"), 0, false, 30,
                (30, CpAnimEventT.Finished)),
            CpFlyingHeadAnimInfoT.atk_DashAtk_Windup => new(
                Animator.StringToHash("CpFlyingHeadVis_Atk_Dash_Windup"), 0, false, 34,
                (34, CpAnimEventT.Finished)),
            CpFlyingHeadAnimInfoT.idle => new(
                Animator.StringToHash("CpFlyingHeadVis_Idle"), 0, true, 50),
            CpFlyingHeadAnimInfoT.knockback_Weak_Bwd => new(
                Animator.StringToHash("CpFlyingHeadVis_Knockback_Weak_Bwd"), 0, false, 15,
                (15, CpAnimEventT.Finished)),
            CpFlyingHeadAnimInfoT.knockback_Weak_Fwd => new(
                Animator.StringToHash("CpFlyingHeadVis_Knockback_Weak_Fwd"), 0, false, 15,
                (15, CpAnimEventT.Finished)),
            _ => GeneralUtils.LogErrorForInput<CpFlyingHeadAnimInfoT, AnimInfo>(t)
        };
    }
}
