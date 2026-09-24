// Add all public enums here.

public enum AtkPhase : byte {
    Windup,
    Impact,
    Recovery
}

public enum BtNodeMethodCall : byte {
    None,
    Cond_IsInAggroRange,
    Cond_IsInAtkRange,
    Cmd_Atk,
}

public enum BtNodeT : byte {
    Cmd_Atk1,
    Cmd_Idle,
    Cmd_MovToTgt,
    Cond_InAggroRange,
    Cond_InAtkRange,
    Selector,
    Sequence,
}

public enum BtResult : byte {
    Success,
    Failure,
    Running,
}

public enum BtT {
    FollowNAttack,
    Idle,
}

public enum BufferableInput : byte {
    None,
    RShldr,
    RTrg,
    LShldr,
    BtnE
}

public enum ComboNodeConfigT : byte {
    BasicImpact,
    BasicRecovery,
    BasicBranch,
    ShootProj,
}

/// <summary>
/// Animation events used by the capsule pawn Animator.
/// </summary>
public enum CpAnimEventT {
    // NOTE: This doesn't mean that the character is grounded, only that the animation reached the point
    // NOTE C: where the vertical movement is no more controlled by the animation. (6.9.2026)
    AirtimeEnded,
    AirtimeStarted,
    BufferedInputStSwitchAllowed,
    ComboAllowed,
    ComboDisallowed,
    DodgeAllowed,
    Finished,
    HitDealerActivated,
    HitDealerDeactivated,
    InvulEnd,
    YawDisallowed,
    YawAllowed,
}

/// <summary>
/// All animation states of capsule pawn.
/// </summary>
public enum CpAnimInfoT {
    atk_FlyingAtk_Impact,
    atk_FlyingAtk_Recovery,
    atk_FlyingAtk_Windup,
    atk_GunShoot_Recovery,
    atk_GunShoot_Windup,
    atk_HorSlash0_Impact,
    atk_HorSlash0_Recovery,
    atk_HorSlash0_Windup,
    atk_HorSlash1_Impact,
    atk_HorSlash1_Recovery,
    atk_HorSlash2_Impact,
    atk_JumpVerSlam,
    dodge,
    falling,
    fallLanding,
    idle,
    knockback_Weak_Bwd,
    knockback_Weak_Fwd,
    walk
}

public enum Dir2DHor : byte {
    Left,
    Right,
    Backward,
    Forward,
}
public enum Dir2DVer : byte {

    Left,
    Right,
    Down,
    Up,
}

public enum Dir3D : byte {
    Left,
    Right,
    Down,
    Up,
    Backward,
    Forward,
}

public enum Faces : byte {
    Left,
    Right,
    Bottom,
    Top,
    Near,
    Far,
}

/// <summary>
/// Used to decide how the direction of a hit is calculated.
/// </summary>
public enum HitDirMode : byte {
    /// <summary>
    /// Calculates direction from <see cref="HitData.sourceTrf"/>
    /// </summary>
    FromHitSourceTrfToHitReciever,
    /// <summary>
    /// Calculates direction from <see cref="HitData.wldDir"/>
    /// </summary>
    WldDir,
}

public enum HpBarTrailingSt {
    DelayingDecrease,
    Decreasing,
    Settled,
}

public enum KnockbackT : byte {
    None,
    Weak,
    Strong
}

public enum PawnTeam : byte {
    /// <summary>
    /// Default local player team.
    /// </summary>
    Team0,
    /// <summary>
    /// Default ai enemy team
    /// </summary>
    Team1,
    Team2,
    Team3,
    EnemyToAll,
    FriendToAll,
}

public enum Side : byte {
    Left,
    Right,
}
