// Add all public enums here.


public enum AtkPhase{
    Windup,
    Impact,
    Recovery
}

public enum BufferableInput {
    None,
    Atk_Light,
    Atk_Heavy,
    Atk_Ult,
    Dodge
}

/// <summary>
/// Capsule pawn action state. These pair animations and animation events with gameplay logic.
/// </summary>
public enum CpActSt {
    Atk_FlyingAtk,
    // TODO: start indexing from 0, ugh.
    Atk_HorSlash1,
    Atk_HorSlash2,
    Atk_HorSlash3,
    Atk_Jump,
    Atk_ShootHomingProj,
    Dodge,
    Falling,
    FallLanding,
    Idle,
    Knockback_Weak,
    Walk
}

/// <summary>
/// Animation events used by the capsule pawn Animator.
/// </summary>
public enum CpAnimEventT {
    Atk_GunShoot_Recovery_Finished,
    Atk_GunShoot_Windup_Finished,
    Dodge_BufferedInputStSwitchAllowed,
    Dodge_Finished,
    Dodge_InvulEnd,
    Dodge_YawAllowed,
    FallLanding_CanSwitchSt,
    FallLanding_Finished,
    Atk_FlyingAtk_Impact_HitDealerActivated,
    FlyingAtk_Impact_Finished,
    Atk_FlyingAtk_Recovery_Finished,
    Atk_FlyingAtk_Windup_Finished,
    Atk_HorSlash1_Impact_RotationAllowed,
    Atk_HorSlash1_Impact_RotationDisallowed,
    Atk_HorSlash1_Impact_HitDealerActivated,
    Atk_HorSlash1_Impact_HitDealerDeactivated,
    Atk_HorSlash1_Impact_ComboAllowed,
    Atk_HorSlash1_Impact_ComboDisallowed,
    Atk_HorSlash1_Impact_Finished,
    Atk_HorSlash1_Recovery_DodgeAllowed,
    Atk_HorSlash1_Recovery_Finished,
    Atk_HorSlash1_Windup_Finished,
    Atk_HorSlash2_Impact_ComboAllowed,
    Atk_HorSlash2_Impact_ComboDisallowed,
    Atk_HorSlash2_Impact_Finished,
    Atk_HorSlash2_Impact_HitDealerActivated,
    Atk_HorSlash2_Impact_HitDealerDeactivated,
    Atk_HorSlash2_Impact_RotationAllowed,
    Atk_HorSlash2_Impact_RotationDisallowed,
    Atk_HorSlash2_Recovery_DodgeAllowed,
    Atk_HorSlash2_Recovery_Finished,
    Atk_HorSlash3_Impact_ComboAllowed,
    Atk_HorSlash3_Impact_ComboDisallowed,
    Atk_HorSlash3_Impact_Finished,
    Atk_HorSlash3_Impact_HitDealerActivated,
    Atk_HorSlash3_Impact_HitDealerDeactivated,
    Atk_HorSlash3_Impact_RotationAllowed,
    Atk_HorSlash3_Impact_RotationDisallowed,
    Atk_JumpVerSlam_Finished,
    Atk_JumpVerSlam_HitboxActivated,
    Atk_JumpVerSlam_HitboxDeactivated,
    Atk_JumpVerSlam_JumpFinished,
    Atk_JumpVerSlam_JumpStarted,
    Knockback_Weak_Bwd_Finished,
    Knockback_Weak_Fwd_Finished
}

public enum BtNodeT {
    Cmd_Atk1,
    Cmd_Idle,
    Cmd_MovToTgt,
    Cond_InAggroRange,
    Cond_InAtkRange,
    Selector,
    Sequence,
}

public enum BtResult {
    Success,
    Failure,
    Running,
}

public enum Directions2DVertical{
    Left,
    Right,
    Down,
    Up,
}

public enum Directions2DHorizontal{
    Left,
    Right,
    Backward,
    Forward,
}

public enum Directions3D{
    Left,
    Right,
    Down,
    Up,
    Backward,
    Forward,
}

public enum BtNodeMethodCall {
    None,
    Cond_IsInAggroRange,
    Cond_IsInAtkRange,
    Cmd_Atk,
}

public enum Faces{
    Left,
    Right,
    Bottom,
    Top,
    Near,
    Far,
}

public enum KnockbackT {
    None,
    Weak,
    Strong
}

public enum LocomotionType {
    VelocityByDirectionalInput,
    DirectMotion,
}

public enum Side{
    Left,
    Right,
}

public enum HandEquippableT {
    Empty,
    Sword,
    Hammer,
    Pistol,
}
