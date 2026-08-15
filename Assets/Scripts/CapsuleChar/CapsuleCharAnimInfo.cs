using UnityEngine;

/// <summary>
/// Info about capsule character animation states used by <see cref="AnimEventPlr"/>.
/// </summary>
public static class CapsuleCharAnimInfo {
    public static AnimInfo atk_FlyingAtk_Impact = new(
        Animator.StringToHash("PC_Atk_FlyingAtk_Impact"),
        0,
        false,
        30
    );

    public static AnimInfo atk_FlyingAtk_Recovery = new(
        Animator.StringToHash("PC_Atk_FlyingAtk_Recovery"),
        0,
        false,
        48
    );

    public static AnimInfo atk_FlyingAtk_Windup = new(
        Animator.StringToHash("PC_Atk_FlyingAtk_Windup"),
        0,
        false,
        95
    );

    public static AnimInfo atk_HorSlash1_Impact = new(
        Animator.StringToHash("PC_Atk_HorSlash1_Impact"),
        0,
        false,
        26
    );

    public static AnimInfo atk_HorSlash1_Recovery = new(
        Animator.StringToHash("PC_Atk_HorSlash1_Recovery"),
        0,
        false,
        18
    );

    public static AnimInfo atk_HorSlash1_Windup = new(
        Animator.StringToHash("PC_Atk_HorSlash1_Windup"),
        0,
        false,
        26
    );

    public static AnimInfo atk_HorSlash2_Impact = new(
        Animator.StringToHash("PC_Atk_HorSlash2_Impact"),
        0,
        false,
        26
    );

    public static AnimInfo atk_HorSlash2_Recovery = new(
        Animator.StringToHash("PC_Atk_HorSlash2_Recovery"),
        0,
        false,
        18
    );

    public static AnimInfo atk_HorSlash3_Impact = new(
        Animator.StringToHash("PC_Atk_HorSlash3_Impact"),
        0,
        false,
        26
    );

    public static AnimInfo atk_JumpVerSlam = new(
        Animator.StringToHash("PC_Atk_JumpVerSlam"),
        0,
        false,
        122
    );

    public static AnimInfo dodge = new(
        Animator.StringToHash("PC_Dodge"),
        0,
        false,
        30
    );

    public static AnimInfo idle = new(
        Animator.StringToHash("PC_Idle"),
        0,
        true,
        90
    );

    public static AnimInfo walk = new(
        Animator.StringToHash("PC_Walk"),
        0,
        true,
        20
    );
}
