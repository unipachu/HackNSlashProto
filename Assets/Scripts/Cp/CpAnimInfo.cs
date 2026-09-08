using System;
using UnityEngine;

// TODO: Move to enum file.
public enum CpAnimInfoT {
    atk_FlyingAtk_Impact,
    atk_FlyingAtk_Recovery,
    atk_FlyingAtk_Windup,
    atk_GunShoot_Recovery,
    atk_GunShoot_Windup,
    atk_HorSlash1_Impact,
    atk_HorSlash1_Recovery,
    atk_HorSlash1_Windup,
    atk_HorSlash2_Impact,
    atk_HorSlash2_Recovery,
    atk_HorSlash3_Impact,
    atk_JumpVerSlam,
    dodge,
    falling,
    fallLanding,
    idle,
    knockback_Weak_Bwd,
    knockback_Weak_Fwd,
    walk
}

/// <summary>
/// Info about capsule pawn animation states used by <see cref="AnimEventPlr"/>.
/// </summary>
public static class CpAnimInfo {
    public static AnimInfo Get(CpAnimInfoT t) {
        return t switch {
            CpAnimInfoT.atk_FlyingAtk_Impact => new(Animator.StringToHash("Cc_Atk_FlyingAtk_Impact"), 0, false, 15, (13, CpAnimEventT.HitDealerActivated), (15, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_FlyingAtk_Recovery => new(Animator.StringToHash("Cc_Atk_FlyingAtk_Recovery"), 0, false, 24, (24, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_FlyingAtk_Windup => new(Animator.StringToHash("Cc_Atk_FlyingAtk_Windup"), 0, false, 47, (47, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_GunShoot_Recovery => new(Animator.StringToHash("Cc_Atk_GunShoot_Recovery"), 0, false, 40, (40, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_GunShoot_Windup => new(Animator.StringToHash("Cc_Atk_GunShoot_Windup"), 0, false, 30, (30, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_HorSlash1_Impact => new(Animator.StringToHash("Cc_Atk_HorSlash1_Impact"), 0, false, 13, (0, CpAnimEventT.YawAllowed), (4, CpAnimEventT.YawDisallowed), (4, CpAnimEventT.HitDealerActivated), (9, CpAnimEventT.HitDealerDeactivated), (11, CpAnimEventT.ComboAllowed), (12, CpAnimEventT.ComboDisallowed), (13, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_HorSlash1_Recovery => new(Animator.StringToHash("Cc_Atk_HorSlash1_Recovery"), 0, false, 9, (3, CpAnimEventT.DodgeAllowed), (9, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_HorSlash1_Windup => new(Animator.StringToHash("Cc_Atk_HorSlash1_Windup"), 0, false, 9, (9, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_HorSlash2_Impact => new(Animator.StringToHash("Cc_Atk_HorSlash2_Impact"), 0, false, 13, (0, CpAnimEventT.YawAllowed), (4, CpAnimEventT.YawDisallowed), (4, CpAnimEventT.HitDealerActivated), (9, CpAnimEventT.HitDealerDeactivated), (11, CpAnimEventT.ComboAllowed), (12, CpAnimEventT.ComboDisallowed), (13, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_HorSlash2_Recovery => new(Animator.StringToHash("Cc_Atk_HorSlash2_Recovery"), 0, false, 9, (3, CpAnimEventT.DodgeAllowed), (9, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_HorSlash3_Impact => new(Animator.StringToHash("Cc_Atk_HorSlash3_Impact"), 0, false, 13, (0, CpAnimEventT.YawAllowed), (4, CpAnimEventT.YawDisallowed), (4, CpAnimEventT.HitDealerActivated), (9, CpAnimEventT.HitDealerDeactivated), (11, CpAnimEventT.ComboAllowed), (12, CpAnimEventT.ComboDisallowed), (13, CpAnimEventT.Finished)),
            CpAnimInfoT.atk_JumpVerSlam => new(Animator.StringToHash("Cc_Atk_JumpVerSlam"), 0, false, 61, (20, CpAnimEventT.AirtimeStarted), (36, CpAnimEventT.HitDealerActivated), (40, CpAnimEventT.AirtimeEnded), (44, CpAnimEventT.HitDealerDeactivated), (61, CpAnimEventT.Finished)),
            CpAnimInfoT.dodge => new(Animator.StringToHash("Cc_Dodge"), 0, false, 18, (3, CpAnimEventT.YawAllowed), (13, CpAnimEventT.InvulEnd), (17, CpAnimEventT.BufferedInputStSwitchAllowed), (18, CpAnimEventT.Finished)),
            CpAnimInfoT.falling => new(Animator.StringToHash("Cc_Falling"), 0, true, 40),
            CpAnimInfoT.fallLanding => new(Animator.StringToHash("Cc_FallLanding"), 0, false, 30, (5, CpAnimEventT.DodgeAllowed), (30, CpAnimEventT.Finished)),
            CpAnimInfoT.idle => new(Animator.StringToHash("Cc_Idle"), 0, true, 45),
            CpAnimInfoT.knockback_Weak_Bwd => new(Animator.StringToHash("Cc_Knockback_Weak_Bwd"), 0, false, 18, (18, CpAnimEventT.Finished)),
            CpAnimInfoT.knockback_Weak_Fwd => new(Animator.StringToHash("Cc_Knockback_Weak_Fwd"), 0, false, 18, (18, CpAnimEventT.Finished)),
            CpAnimInfoT.walk => new(Animator.StringToHash("Cc_Walk"), 0, true, 10),
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
        };
    }
}
