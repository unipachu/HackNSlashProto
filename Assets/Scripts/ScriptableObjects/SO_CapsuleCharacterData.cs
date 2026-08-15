using UnityEngine;

[CreateAssetMenu(fileName = "PCData", menuName = "Character Data/PC Data")]
public class SO_CapsuleCharacterData : ScriptableObject{
    [Header("Playable character read-only base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("General Movement Settings")]
    [Tooltip("In m/s^2. Should be around 9.81.")]
    [SerializeField] public float gravitationalAcc = 20f;
    [Tooltip("How long should inputs stay in the buffer?")]
    [SerializeField] public float inputBufferDuration = 0.3f;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    [SerializeField] public float st_AtkJump_DownSpeedAfterJumpFinished = 10f;

    // TODO: Do you even use the movement settings for the attacks? Also these are the input based
    // TODO C: speeds, not anim root motion speeds.
    [Header("St_AtkHorSlash")]
    //[field: SerializeField] public float St_AtkHorSlash1_LinAcc { get; private set; } = 50f;
    //[field: SerializeField] public int St_AtkHorSlash1_MaxLinSpd { get; private set; } = 2;
    //[field: SerializeField] public float St_AtkHorSlash1_MaxAngSpd { get; private set; } = 200f;
    [Tooltip("NOTE: This should be shorter than the recovery animation dur.")]
    [SerializeField] public float st_AtkHorSlash_RecoveryMotionInterpDur = 0.2f;
    [SerializeField] public float st_AtkHorSlash_Impact_AngSpd = 800f;

    [Header("St_Dodge")]
    [SerializeField] public float st_Dodge_YawAngSpd = 400f;

    [Header("St_Walk")]
    [SerializeField] public float st_Walk_LinAcc = 100f;
    [SerializeField] public int st_Walk_MaxLinSpd = 5;
    [SerializeField] public float st_Walk_MaxAngSpd = 800f;

    public CapsuleCharacterConfig ToStruct() => new CapsuleCharacterConfig {
        gravitationalAcc = gravitationalAcc,
        inputBufferDuration = inputBufferDuration,
        st_AtkJump_DownSpeedAfterJumpFinished = st_AtkJump_DownSpeedAfterJumpFinished,
        st_AtkHorSlash_RecoveryMotionInterpDur = st_AtkHorSlash_RecoveryMotionInterpDur,
        st_AtkHorSlash_Impact_AngSpd = st_AtkHorSlash_Impact_AngSpd,
        st_Dodge_YawAngSpd = st_Dodge_YawAngSpd,
        st_Walk_LinAcc = st_Walk_LinAcc,
        st_Walk_MaxLinSpd = st_Walk_MaxLinSpd,
        st_Walk_MaxAngSpd = st_Walk_MaxAngSpd
    };
}

public struct CapsuleCharacterConfig {
    public float gravitationalAcc;
    public float inputBufferDuration;
    public float st_AtkJump_DownSpeedAfterJumpFinished;
    public float st_AtkHorSlash_RecoveryMotionInterpDur;
    public float st_AtkHorSlash_Impact_AngSpd;
    public float st_Dodge_YawAngSpd;
    public float st_Walk_LinAcc;
    public float st_Walk_MaxLinSpd;
    public float st_Walk_MaxAngSpd;
}