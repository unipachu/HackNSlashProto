using UnityEngine;

/// <summary>
/// Info about capsule pawn animation states used by <see cref="AnimEventPlr"/>.
/// </summary>
public static class CpAnimInfo {
    public static AnimInfo atk_FlyingAtk_Impact = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Impact"),
        0,
        false,
        15,
        (13, CpAnimEventT.HitDealerActivated),
        (15, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_FlyingAtk_Recovery = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Recovery"),
        0,
        false,
        24,
        (24, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_FlyingAtk_Windup = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Windup"),
        0,
        false,
        47,
        (47, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_GunShoot_Recovery = new(
        Animator.StringToHash("Cc_Atk_GunShoot_Recovery"),
        0,
        false,
        40,
        (40, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_GunShoot_Windup = new(
        Animator.StringToHash("Cc_Atk_GunShoot_Windup"),
        0,
        false,
        30,
        (30, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_HorSlash1_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Impact"),
        0,
        false,
        13,
        (0, CpAnimEventT.YawAllowed),
        (4, CpAnimEventT.YawDisallowed),
        (4, CpAnimEventT.HitDealerActivated),
        (9, CpAnimEventT.HitDealerDeactivated),
        (11, CpAnimEventT.ComboAllowed),
        (12, CpAnimEventT.ComboDisallowed),
        (13, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_HorSlash1_Recovery = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Recovery"),
        0,
        false,
        9,
        (3, CpAnimEventT.DodgeAllowed),
        (9, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_HorSlash1_Windup = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Windup"),
        0,
        false,
        9,
        (9, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_HorSlash2_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash2_Impact"),
        0,
        false,
        13,
        (0, CpAnimEventT.YawAllowed),
        (4, CpAnimEventT.YawDisallowed),
        (4, CpAnimEventT.HitDealerActivated),
        (9, CpAnimEventT.HitDealerDeactivated),
        (11, CpAnimEventT.ComboAllowed),
        (12, CpAnimEventT.ComboDisallowed),
        (13, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_HorSlash2_Recovery = new(
        Animator.StringToHash("Cc_Atk_HorSlash2_Recovery"),
        0,
        false,
        9,
        (3, CpAnimEventT.DodgeAllowed),
        (9, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_HorSlash3_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash3_Impact"),
        0,
        false,
        13,
        (0, CpAnimEventT.YawAllowed),
        (4, CpAnimEventT.YawDisallowed),
        (4, CpAnimEventT.HitDealerActivated),
        (9, CpAnimEventT.HitDealerDeactivated),
        (11, CpAnimEventT.ComboAllowed),
        (12, CpAnimEventT.ComboDisallowed),
        (13, CpAnimEventT.Finished)
    );

    public static AnimInfo atk_JumpVerSlam = new(
        Animator.StringToHash("Cc_Atk_JumpVerSlam"),
        0,
        false,
        61,
        (20, CpAnimEventT.AirtimeStarted),
        (36, CpAnimEventT.HitDealerActivated),
        (40, CpAnimEventT.AirtimeEnded),
        (44, CpAnimEventT.HitDealerDeactivated),
        (61, CpAnimEventT.Finished)
    );

    public static AnimInfo dodge = new(
        Animator.StringToHash("Cc_Dodge"),
        0,
        false,
        18,
        (3, CpAnimEventT.YawAllowed),
        (13, CpAnimEventT.InvulEnd),
        (17, CpAnimEventT.BufferedInputStSwitchAllowed),
        (18, CpAnimEventT.Finished)
    );

    public static AnimInfo falling = new(
        Animator.StringToHash("Cc_Falling"),
        0,
        true,
        40
    );

    public static AnimInfo fallLanding = new(
        Animator.StringToHash("Cc_FallLanding"),
        0,
        false,
        30,
        (5, CpAnimEventT.DodgeAllowed),
        (30, CpAnimEventT.Finished)
    );

    public static AnimInfo idle = new(
        Animator.StringToHash("Cc_Idle"),
        0,
        true,
        45
    );

    public static AnimInfo knockback_Weak_Bwd = new(
        Animator.StringToHash("Cc_Knockback_Weak_Bwd"),
        0,
        false,
        18,
        (18, CpAnimEventT.Finished)
    );

    public static AnimInfo knockback_Weak_Fwd = new(
        Animator.StringToHash("Cc_Knockback_Weak_Fwd"),
        0,
        false,
        18,
        (18, CpAnimEventT.Finished)
    );

    public static AnimInfo walk = new(
        Animator.StringToHash("Cc_Walk"),
        0,
        true,
        10
    );
}