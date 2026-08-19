using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "CapsuleCharData", menuName = "Character Data/CapsuleChar Data")]
public class So_CapsuleCharData : ScriptableObject{
    [Header("Playable character read-only base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("General Movement Settings")]
    [Tooltip("In m/s^2. Should be around 9.81.")]
    [SerializeField] float gravitationalAcc = 10;
    [Tooltip("If character succeeds isGrounded check, this is the vertical velocity "
        + "used to snap slightly hovering character to the ground.")]
    [SerializeField] float groundSnapVerDownSpd = 1000;
    [Tooltip("Max vertical speed when in falling state.")]
    [SerializeField] float maxFallSpd = 30;

    [Header("Input Settings")]
    [Tooltip("How long should inputs stay in the buffer (in sec)?")]
    [SerializeField] float inputBufferDuration = 0.3f;

    [Header("Health")]
    [SerializeField] int maxHP = 100;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    [SerializeField] float st_AtkJump_DownSpeedAfterJumpFinished = 10;

    [Header("St_AtkHorSlash")]
    [SerializeField] float st_AtkHorSlash_Windup_MaxAngSpd = 1000;
    [SerializeField] float st_AtkHorSlash_Impact_AngSpd = 1200;
    [Tooltip("NOTE: This should be shorter than the recovery animation dur.")]
    [SerializeField] float st_AtkHorSlash_RecoveryMotionInterpDur = 0.2f;

    [Header("St_Falling")]
    [Tooltip("The distance the character needs to fall to enter landing animation when "
        + "hitting the ground.")]
    [SerializeField] float st_Falling_LandingStFallDistThreshold = 3;
    // TODO: Probably should be called "horizontal input based acceleration". Same for
    // TODO C: other params like this.
    [SerializeField] float st_Falling_LinAcc = 10;
    [SerializeField] float st_Falling_MaxLinSpd = 1;

    [Header("St_Dodge")]
    [SerializeField] float st_Dodge_YawAngSpd = 400;

    [Header("St_Walk")]
    [SerializeField] float st_Walk_LinAcc = 100;
    [SerializeField] int st_Walk_MaxLinSpd = 5;
    [SerializeField] float st_Walk_MaxAngSpd = 1000;

    // NOTE: This dumb conversion seems to be only way to have default inspector values since
    // NOTE C: C# 9 doesn't support default struct values. Otherwise I'd just have a serialized
    // NOTE C: struct directly in the scriptable object.
    public CapsuleCharData ToStruct() => new CapsuleCharData {
        curStDur = 0,
        gravitationalAcc = gravitationalAcc,
        groundCastHitSomething = false,
        groundCastNrm = float3.zero,
        groundSnapVerDownSpd = groundSnapVerDownSpd,
        hp_Cur = maxHP,
        hp_Max = maxHP,
        inputBufferDur = inputBufferDuration,
        invul = false,
        isAffectedByGravity = true,
        isGrounded = false,
        lastCharCtrlVel = float3.zero,
        lastRecievedHitDir = float3.zero,
        maxFallSpd = maxFallSpd,
        st_AtkHorSlash_RecoveryMotionInterpDur = st_AtkHorSlash_RecoveryMotionInterpDur,
        st_AtkHorSlash_Impact_AngSpd = st_AtkHorSlash_Impact_AngSpd,
        st_AtkHorSlash_Windup_MaxAngSpd = st_AtkHorSlash_Windup_MaxAngSpd,
        st_AtkJump_DownSpeedAfterJumpFinished = st_AtkJump_DownSpeedAfterJumpFinished,
        st_Dodge_YawSpd = st_Dodge_YawAngSpd,
        st_Falling_LandingStFallDistThreshold = st_Falling_LandingStFallDistThreshold,
        st_Falling_LinAcc = st_Falling_LinAcc,
        st_Falling_MaxLinSpd = st_Falling_MaxLinSpd,
        st_Walk_LinAcc = st_Walk_LinAcc,
        st_Walk_MaxLinSpd = st_Walk_MaxLinSpd,
        st_Walk_YawSpd = st_Walk_MaxAngSpd,
        vel_Hor = float2.zero,
        vel_Ver = 0,
        vel_Yaw = 0
    };
}
