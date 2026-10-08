using UnityEngine;

/// <summary>
/// Info about capsule pawn animation states used by <see cref="AnimEventPlr"/>.
/// </summary>
// TODO: Rename to CpHumanoidAnimInfoFactory
public static class CpAnimInfoFactory {
    public static AnimInfo Construct(CpHumanoidAnimInfoT t) {
        return t switch {
            CpHumanoidAnimInfoT.atk_FlyingAtk_Impact => new(
                Animator.StringToHash("Cc_Atk_FlyingAtk_Impact"), 0, false, 15,
                (13, CpAnimEventT.HitDealerActivated),
                (15, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_FlyingAtk_Recovery => new(
                Animator.StringToHash("Cc_Atk_FlyingAtk_Recovery"), 0, false, 24,
                (24, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_FlyingAtk_Windup => new(
                Animator.StringToHash("Cc_Atk_FlyingAtk_Windup"), 0, false, 47,
                (47, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_GunShoot_AimPose => new(
                Animator.StringToHash("Cc_Atk_GunShoot_AimPose"), 0, true, 40),
            CpHumanoidAnimInfoT.atk_GunShoot_Recovery => new(
                Animator.StringToHash("Cc_Atk_GunShoot_Recovery"), 0, false, 40,
                (28, CpAnimEventT.InputMovAllowed),
                (40, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_GunShoot_Windup => new(
                Animator.StringToHash("Cc_Atk_GunShoot_Windup"), 0, false, 30,
                (30, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_HorSlash0_Impact => new(
                Animator.StringToHash("Cc_Atk_HorSlash0_Impact"), 0, false, 13,
                (0, CpAnimEventT.YawAllowed),
                (4, CpAnimEventT.YawDisallowed),
                (4, CpAnimEventT.HitDealerActivated),
                (11, CpAnimEventT.ComboAllowed),
                (12, CpAnimEventT.HitDealerDeactivated),
                (12, CpAnimEventT.ComboDisallowed),
                (13, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_HorSlash0_Recovery => new(
                Animator.StringToHash("Cc_Atk_HorSlash0_Recovery"), 0, false, 9,
                (0, CpAnimEventT.InputMovAllowed),
                (3, CpAnimEventT.DodgeAllowed),
                (9, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_HorSlash0_Windup => new(
                Animator.StringToHash("Cc_Atk_HorSlash0_Windup"), 0, false, 9,
                (9, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_HorSlash1_Impact => new(
                Animator.StringToHash("Cc_Atk_HorSlash1_Impact"), 0, false, 13,
                (0, CpAnimEventT.YawAllowed),
                (4, CpAnimEventT.YawDisallowed),
                (4, CpAnimEventT.HitDealerActivated),
                (11, CpAnimEventT.ComboAllowed),
                (12, CpAnimEventT.HitDealerDeactivated),
                (12, CpAnimEventT.ComboDisallowed),
                (13, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_HorSlash1_Recovery => new(
                Animator.StringToHash("Cc_Atk_HorSlash1_Recovery"), 0, false, 9,
                (0, CpAnimEventT.InputMovAllowed),
                (3, CpAnimEventT.DodgeAllowed),
                (9, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_HorSlash2_Impact => new(
                Animator.StringToHash("Cc_Atk_HorSlash2_Impact"), 0, false, 13,
                (0, CpAnimEventT.YawAllowed),
                (4, CpAnimEventT.YawDisallowed),
                (4, CpAnimEventT.HitDealerActivated),
                (11, CpAnimEventT.ComboAllowed),
                (12, CpAnimEventT.HitDealerDeactivated),
                (12, CpAnimEventT.ComboDisallowed),
                (13, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_JumpVerSlam => new(
                Animator.StringToHash("Cc_Atk_JumpVerSlam"), 0, false, 61,
                (20, CpAnimEventT.AirtimeStarted),
                (36, CpAnimEventT.HitDealerActivated),
                (40, CpAnimEventT.AirtimeEnded),
                (44, CpAnimEventT.HitDealerDeactivated),
                (61, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_VerSlash0_Impact => new(
                Animator.StringToHash("Cc_Atk_VerSlash0_Impact"), 0, false, 13,
                (2, CpAnimEventT.HitDealerActivated),
                (7, CpAnimEventT.HitDealerDeactivated),
                (7, CpAnimEventT.ComboAllowed),
                (8, CpAnimEventT.ComboDisallowed),
                (13, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_VerSlash0_Recovery => new(
                Animator.StringToHash("Cc_Atk_VerSlash0_Recovery"), 0, false, 22,
                (18, CpAnimEventT.InputMovAllowed),
                (22, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.atk_VerSlash0_Windup => new(
                Animator.StringToHash("Cc_Atk_VerSlash0_Windup"), 0, false, 30,
                (30, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.dodge => new(
                Animator.StringToHash("Cc_Dodge"), 0, false, 18,
                (3, CpAnimEventT.YawAllowed),
                (16, CpAnimEventT.InvulEnd),
                (17, CpAnimEventT.BufferedInputStSwitchAllowed),
                (18, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.falling => new(
                Animator.StringToHash("Cc_Falling"), 0, true, 40),
            CpHumanoidAnimInfoT.fallLanding => new(
                Animator.StringToHash("Cc_FallLanding"), 0, false, 30,
                (5, CpAnimEventT.DodgeAllowed),
                (30, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.idle => new(
                Animator.StringToHash("Cc_Idle"), 0, true, 45),
            CpHumanoidAnimInfoT.knockback_Weak_Bwd => new(
                Animator.StringToHash("Cc_Knockback_Weak_Bwd"), 0, false, 18,
                (18, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.knockback_Weak_Fwd => new(
                Animator.StringToHash("Cc_Knockback_Weak_Fwd"), 0, false, 18,
                (18, CpAnimEventT.Finished)),
            CpHumanoidAnimInfoT.walk => new(
                Animator.StringToHash("Cc_Walk"), 0, true, 10),
            _ => GeneralUtils.LogErrorForInput<CpHumanoidAnimInfoT, AnimInfo>(t)
        };
    }
}
