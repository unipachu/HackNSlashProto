using UnityEngine;

[CreateAssetMenu(fileName = "CpConfig_", menuName = "Scriptable Object Data/CpConfig")]
public class So_CpConfig : ScriptableObject {
    [Header("Capsule pawn base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("Ui")]
    public string displayName = "Test Name";
    
    [Header("General Movement Settings")]
    [Tooltip("If pawn succeeds isGrounded check, this is the vertical velocity "
        + "used to snap slightly hovering pawn to the ground.")]
    public float groundSnapVerDownSpd = 1000;
    public float impact_YawSpd = 1200;
    public float walkHorAcc = 100;
    public float walkTgtHorSpd = 5;
    public float walkYawSpd = 1000;
    public float windup_YawSpd = 1000;

    [Header("Input Settings")]
    [Tooltip("How long should inputs stay in the buffer (in sec)?")]
    public float inputBufferDuration = 0.3f;

    [Header("Health")]
    public int hp_Max = 100;

    [Header("Equipment Settings")]
    public HandItemT rHandItem;

    [Header("Team Settings")]
    public Team team = Team.Team0;

    [Header("Debug")]
    public bool enableDbgMsgs = false;

    [Header("St_AtkFlying")]
    public float act_AtkFlying_TgtHorSpeed = 2;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    public float act_AtkJump_DownSpeedAfterJumpFinished = 10;

    [Header("St_Falling")]
    [Tooltip("The distance the pawn needs to fall to enter landing animation when "
        + "hitting the ground.")]
    public float act_Falling_LandingStFallDistThreshold = 2;
    public float act_Falling_HorAcc = 10;
    [Tooltip("Gives horizontal air control.")]
    public float act_Falling_TgtHorSpd = 1;

    [Header("St_Dodge")]
    public float act_Dodge_YawAngSpd = 400;
    public float act_Dodge_HorMovSpdMult = 1.5f;
}
