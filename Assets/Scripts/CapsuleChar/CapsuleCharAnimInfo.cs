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
        (13, CapsuleCharAnimEvent.FlyingAtk_Impact_HitDealerActivated),
        (15, CapsuleCharAnimEvent.FlyingAtk_Impact_Finished)
    );

    public static AnimInfo atk_FlyingAtk_Recovery = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Recovery"),
        0,
        false,
        24,
        (24, CapsuleCharAnimEvent.FlyingAtk_Recovery_Finished)
    );

    public static AnimInfo atk_FlyingAtk_Windup = new(
        Animator.StringToHash("Cc_Atk_FlyingAtk_Windup"),
        0,
        false,
        47,
        (47, CapsuleCharAnimEvent.FlyingAtk_Windup_Finished)
    );

    public static AnimInfo atk_HorSlash1_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Impact"),
        0,
        false,
        13,
        (0, CapsuleCharAnimEvent.HorSlash1_Impact_RotationAllowed),
        (2, CapsuleCharAnimEvent.HorSlash1_Impact_RotationDisallowed),
        (4, CapsuleCharAnimEvent.HorSlash1_Impact_HitDealerActivated),
        (9, CapsuleCharAnimEvent.HorSlash1_Impact_HitDealerDeactivated),
        (11, CapsuleCharAnimEvent.HorSlash1_Impact_ComboAllowed),
        (12, CapsuleCharAnimEvent.HorSlash1_Impact_ComboDisallowed),
        (13, CapsuleCharAnimEvent.HorSlash1_Impact_Finished)
    );

    public static AnimInfo atk_HorSlash1_Recovery = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Recovery"),
        0,
        false,
        9,
        (3, CapsuleCharAnimEvent.HorSlash1_Recovery_DodgeAllowed),
        (9, CapsuleCharAnimEvent.HorSlash1_Recovery_Finished)
    );

    public static AnimInfo atk_HorSlash1_Windup = new(
        Animator.StringToHash("Cc_Atk_HorSlash1_Windup"),
        0,
        false,
        9,
        (9, CapsuleCharAnimEvent.HorSlash1_Windup_Finished)
    );

    public static AnimInfo atk_HorSlash2_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash2_Impact"),
        0,
        false,
        13,
        (0, CapsuleCharAnimEvent.HorSlash2_Impact_RotationAllowed),
        (2, CapsuleCharAnimEvent.HorSlash2_Impact_RotationDisallowed),
        (4, CapsuleCharAnimEvent.HorSlash2_Impact_HitDealerActivated),
        (9, CapsuleCharAnimEvent.HorSlash2_Impact_HitDealerDeactivated),
        (11, CapsuleCharAnimEvent.HorSlash2_Impact_ComboAllowed),
        (12, CapsuleCharAnimEvent.HorSlash2_Impact_ComboDisallowed),
        (13, CapsuleCharAnimEvent.HorSlash2_Impact_Finished)
    );

    public static AnimInfo atk_HorSlash2_Recovery = new(
        Animator.StringToHash("Cc_Atk_HorSlash2_Recovery"),
        0,
        false,
        9,
        (3, CapsuleCharAnimEvent.HorSlash2_Recovery_DodgeAllowed),
        (9, CapsuleCharAnimEvent.HorSlash2_Recovery_Finished)
    );

    public static AnimInfo atk_HorSlash3_Impact = new(
        Animator.StringToHash("Cc_Atk_HorSlash3_Impact"),
        0,
        false,
        13,
        (0, CapsuleCharAnimEvent.HorSlash3_Impact_RotationAllowed),
        (2, CapsuleCharAnimEvent.HorSlash3_Impact_RotationDisallowed),
        (4, CapsuleCharAnimEvent.HorSlash3_Impact_HitDealerActivated),
        (9, CapsuleCharAnimEvent.HorSlash3_Impact_HitDealerDeactivated),
        (11, CapsuleCharAnimEvent.HorSlash3_Impact_ComboAllowed),
        (12, CapsuleCharAnimEvent.HorSlash3_Impact_ComboDisallowed),
        (13, CapsuleCharAnimEvent.HorSlash3_Impact_Finished)
    );

    public static AnimInfo atk_JumpVerSlam = new(
        Animator.StringToHash("Cc_Atk_JumpVerSlam"),
        0,
        false,
        61,
        (20, CapsuleCharAnimEvent.JumpVerSlam_JumpStarted),
        (36, CapsuleCharAnimEvent.JumpVerSlam_HitboxActivated),
        (40, CapsuleCharAnimEvent.JumpVerSlam_JumpFinished),
        (44, CapsuleCharAnimEvent.JumpVerSlam_HitboxDeactivated),
        (61, CapsuleCharAnimEvent.JumpVerSlam_Finished)
    );

    public static AnimInfo dodge = new(
        Animator.StringToHash("Cc_Dodge"),
        0,
        false,
        18,
        (3, CapsuleCharAnimEvent.Dodge_YawAllowed),
        (13, CapsuleCharAnimEvent.Dodge_InvulEnd),
        (17, CapsuleCharAnimEvent.Dodge_BufferedInputStSwitchAllowed),
        (18, CapsuleCharAnimEvent.Dodge_Finished)
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
        (18, CapsuleCharAnimEvent.Knockback_Weak_Bwd_Finished)
    );

    public static AnimInfo knockback_Weak_Fwd = new(
        Animator.StringToHash("Cc_Knockback_Weak_Fwd"),
        0,
        false,
        18,
        (18, CapsuleCharAnimEvent.Knockback_Weak_Fwd_Finished)
    );

    public static AnimInfo walk = new(
        Animator.StringToHash("Cc_Walk"),
        0,
        true,
        10
    );
}