using UnityEngine;

[CreateAssetMenu(fileName = "CpData_", menuName = "Scriptable Object Data/CpData")]
public class So_CpData : ScriptableObject {
    [Header("Capsule pawn base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("General Movement Settings")]
    [Tooltip("In m/s^2. Should be around 9.81.")]
    public float gravitationalAcc = 10;
    [Tooltip("If pawn succeeds isGrounded check, this is the vertical velocity "
        + "used to snap slightly hovering pawn to the ground.")]
    public float groundSnapVerDownSpd = 1000;
    [Tooltip("Max vertical speed when in falling state.")]
    public float maxFallSpd = 30;

    [Header("Input Settings")]
    [Tooltip("How long should inputs stay in the buffer (in sec)?")]
    public float inputBufferDuration = 0.3f;

    [Header("Health")]
    public int maxHP = 100;

    [Header("Action States")]
    public CpActSt initSt = CpActSt.Idle;

    [Header("Npc Brain")]
    public int brain_AggroRange = 8;
    public int brain_AtkRange = 4;

    [Header("Equipment Settings")]
    public HandEquippableT equip_RHandEquippable;

    [Header("Debug")]
    public bool enableDebugMsgs = false;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    public float st_AtkJump_DownSpeedAfterJumpFinished = 10;

    [Header("St_AtkHorSlash")]
    public float st_AtkHorSlash_Windup_MaxAngSpd = 1000;
    public float st_AtkHorSlash_Impact_AngSpd = 1200;

    [Header("St_Falling")]
    [Tooltip("The distance the pawn needs to fall to enter landing animation when "
        + "hitting the ground.")]
    public float st_Falling_LandingStFallDistThreshold = 3;
    // TODO: Probably should be called "horizontal input based acceleration". Same for
    // TODO C: other params like this.
    public float st_Falling_LinAcc = 10;
    public float st_Falling_MaxLinSpd = 1;

    [Header("St_Dodge")]
    public float st_Dodge_YawAngSpd = 400;

    [Header("St_Walk")]
    public float st_Walk_LinAcc = 100;
    public int st_Walk_MaxLinSpd = 5;
    public float st_Walk_MaxAngSpd = 1000;
}
