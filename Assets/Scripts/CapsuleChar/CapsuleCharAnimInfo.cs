using UnityEngine;

/// <summary>
/// Info about capsule character animation states used by <see cref="AnimEventPlr"/>.
/// </summary>
public static class CapsuleCharAnimInfo {
    public static AnimInfo atk_FlyingAtk_Impact = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Impact"),
        0,
        false,
        15,
        (13, CapsuleCharAnimEventT.Atk_FlyingAtk_Impact_HitDealerActivated),
        (15, CapsuleCharAnimEventT.FlyingAtk_Impact_Finished)
    );

    public static AnimInfo atk_FlyingAtk_Recovery = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Recovery"),
        0,
        false,
        24,
        (24, CapsuleCharAnimEventT.Atk_FlyingAtk_Recovery_Finished)
    );

    public static AnimInfo atk_FlyingAtk_Windup = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Windup"),
        0,
        false,
        47,
        (47, CapsuleCharAnimEventT.Atk_FlyingAtk_Windup_Finished)
    );

    public static AnimInfo atk_GunShoot_Recovery = new(
        Animator.StringToHash("Cc_Atk_GunShoot_Recovery"),
        0,
        false,
        40,
        (40, CapsuleCharAnimEventT.Atk_GunShoot_Recovery_Finished)
    );

    public static AnimInfo atk_GunShoot_Windup = new(
        Animator.StringToHash("Cc_Atk_GunShoot_Windup"),
        0,
        false,
        30,
        (30, CapsuleCharAnimEventT.Atk_GunShoot_Windup_Finished)
    );

    public static AnimInfo atk_HorSlash1_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Impact"),
        0,
        false,
        13,
        (0, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_RotationAllowed),
        (4, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_RotationDisallowed),
        (4, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_HitDealerActivated),
        (9, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_HitDealerDeactivated),
        (11, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_ComboAllowed),
        (12, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_ComboDisallowed),
        (13, CapsuleCharAnimEventT.Atk_HorSlash1_Impact_Finished)
    );

    public static AnimInfo atk_HorSlash1_Recovery = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Recovery"),
        0,
        false,
        9,
        (3, CapsuleCharAnimEventT.Atk_HorSlash1_Recovery_DodgeAllowed),
        (9, CapsuleCharAnimEventT.Atk_HorSlash1_Recovery_Finished)
    );

    public static AnimInfo atk_HorSlash1_Windup = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Windup"),
        0,
        false,
        9,
        (9, CapsuleCharAnimEventT.Atk_HorSlash1_Windup_Finished)
    );

    public static AnimInfo atk_HorSlash2_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash2_Impact"),
        0,
        false,
        13,
        (0, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_RotationAllowed),
        (4, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_RotationDisallowed),
        (4, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_HitDealerActivated),
        (9, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_HitDealerDeactivated),
        (11, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_ComboAllowed),
        (12, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_ComboDisallowed),
        (13, CapsuleCharAnimEventT.Atk_HorSlash2_Impact_Finished)
    );

    public static AnimInfo atk_HorSlash2_Recovery = new(
        Animator.StringToHash("Cc_Atk_HorSlash2_Recovery"),
        0,
        false,
        9,
        (3, CapsuleCharAnimEventT.Atk_HorSlash2_Recovery_DodgeAllowed),
        (9, CapsuleCharAnimEventT.Atk_HorSlash2_Recovery_Finished)
    );

    public static AnimInfo atk_HorSlash3_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash3_Impact"),
        0,
        false,
        13,
        (0, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_RotationAllowed),
        (4, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_RotationDisallowed),
        (4, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_HitDealerActivated),
        (9, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_HitDealerDeactivated),
        (11, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_ComboAllowed),
        (12, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_ComboDisallowed),
        (13, CapsuleCharAnimEventT.Atk_HorSlash3_Impact_Finished)
    );

    public static AnimInfo atk_JumpVerSlam = new(
        Animator.StringToHash("Cc_Atk_JumpVerSlam"),
        0,
        false,
        61,
        (20, CapsuleCharAnimEventT.Atk_JumpVerSlam_JumpStarted),
        (36, CapsuleCharAnimEventT.Atk_JumpVerSlam_HitboxActivated),
        (40, CapsuleCharAnimEventT.Atk_JumpVerSlam_JumpFinished),
        (44, CapsuleCharAnimEventT.Atk_JumpVerSlam_HitboxDeactivated),
        (61, CapsuleCharAnimEventT.Atk_JumpVerSlam_Finished)
    );

    public static AnimInfo dodge = new(
        Animator.StringToHash("Cc_Dodge"),
        0,
        false,
        18,
        (3, CapsuleCharAnimEventT.Dodge_YawAllowed),
        (13, CapsuleCharAnimEventT.Dodge_InvulEnd),
        (17, CapsuleCharAnimEventT.Dodge_BufferedInputStSwitchAllowed),
        (18, CapsuleCharAnimEventT.Dodge_Finished)
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
        (5, CapsuleCharAnimEventT.FallLanding_CanSwitchSt),
        (30, CapsuleCharAnimEventT.FallLanding_Finished)
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
        (18, CapsuleCharAnimEventT.Knockback_Weak_Bwd_Finished)
    );

    public static AnimInfo knockback_Weak_Fwd = new(
        Animator.StringToHash("Cc_Knockback_Weak_Fwd"),
        0,
        false,
        18,
        (18, CapsuleCharAnimEventT.Knockback_Weak_Fwd_Finished)
    );

    public static AnimInfo walk = new(
        Animator.StringToHash("Cc_Walk"),
        0,
        true,
        10
    );
}