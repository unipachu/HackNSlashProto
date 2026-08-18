using UnityEngine;

[CreateAssetMenu(fileName = "CapsuleCharData", menuName = "Character Data/CapsuleChar Data")]
public class So_CapsuleCharData : ScriptableObject{
    [Header("Playable character read-only base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("General Movement Settings")]
    [Tooltip("In m/s^2. Should be around 9.81.")]
    [SerializeField] float gravitationalAcc = 20;
    [Tooltip("How long should inputs stay in the buffer?")]
    [SerializeField] float inputBufferDuration = 0.3f;

    [Header("Heath")]
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
        gravitationalAcc = gravitationalAcc,
        groundCastHitSomething = false,
        hp_Cur = maxHP,
        hp_Max = maxHP,
        inputBufferDur = inputBufferDuration,
        invul = false,
        isAffectedByGravity = true,
        isGrounded = false,
        lastRecievedHitDir = Vector3.zero,
        st_AtkHorSlash_RecoveryMotionInterpDur = st_AtkHorSlash_RecoveryMotionInterpDur,
        st_AtkHorSlash_Impact_AngSpd = st_AtkHorSlash_Impact_AngSpd,
        st_AtkHorSlash_Windup_MaxAngSpd = st_AtkHorSlash_Windup_MaxAngSpd,
        st_AtkJump_DownSpeedAfterJumpFinished = st_AtkJump_DownSpeedAfterJumpFinished,
        st_Dodge_YawSpd = st_Dodge_YawAngSpd,
        st_Falling_LinAcc = st_Falling_LinAcc,
        st_Falling_MaxLinSpd = st_Falling_MaxLinSpd,
        st_Walk_LinAcc = st_Walk_LinAcc,
        st_Walk_MaxLinSpd = st_Walk_MaxLinSpd,
        st_Walk_YawSpd = st_Walk_MaxAngSpd,
        vel_hor = Vector2.zero,
        vel_ver = 0,
        vel_yaw = 0
    };
}
