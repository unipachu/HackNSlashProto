using UnityEngine;

[CreateAssetMenu(fileName = "CpData_", menuName = "Scriptable Object Data/CpData")]
public class So_CpData : ScriptableObject {
    [Header("Capsule pawn base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("General Movement Settings")]
    [Tooltip("If pawn succeeds isGrounded check, this is the vertical velocity "
        + "used to snap slightly hovering pawn to the ground.")]
    public float groundSnapVerDownSpd = 1000;
    public float impact_YawSpd = 1200;
    public float walkHorAcc = 100;
    public int walkTgtHorSpd = 5;
    public float walkYawSpd = 1000;

    [Header("Input Settings")]
    [Tooltip("How long should inputs stay in the buffer (in sec)?")]
    public float inputBufferDuration = 0.3f;

    [Header("Health")]
    public int maxHP = 100;

    [Header("Npc Brain")]
    public int brain_AggroRange = 8;
    public int brain_AtkRange = 4;

    [Header("Equipment Settings")]
    public HandItemT rHandItem;

    [Header("Debug")]
    public bool enableDebugMsgs = false;

    [Header("St_AtkFlying")]
    public float st_AtkFlying_TgtHorSpeed = 2;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    public float st_AtkJump_DownSpeedAfterJumpFinished = 10;

    [Header("St_AtkHorSlash")]
    public float st_AtkHorSlash_Windup_YawSpd = 1000;

    [Header("St_Falling")]
    [Tooltip("The distance the pawn needs to fall to enter landing animation when "
        + "hitting the ground.")]
    public float st_Falling_LandingStFallDistThreshold = 2;
    public float st_Falling_HorAcc = 10;
    [Tooltip("Gives horizontal air control.")]
    public float st_Falling_TgtHorSpd = 1;

    [Header("St_Dodge")]
    public float st_Dodge_YawAngSpd = 400;
}
