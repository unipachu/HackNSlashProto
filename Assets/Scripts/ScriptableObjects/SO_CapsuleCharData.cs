using UnityEngine;

[CreateAssetMenu(fileName = "PCData", menuName = "Character Data/PC Data")]
public class SO_CapsuleCharData : ScriptableObject{
    [Header("Playable character read-only base data."
        + "\nNOTE: State specific data starts with \"St_[state name]_\".")]

    [Header("General Movement Settings")]
    [Tooltip("In m/s^2. Should be around 9.81.")]
    [SerializeField] float gravitationalAcc = 20f;
    [Tooltip("How long should inputs stay in the buffer?")]
    [SerializeField] float inputBufferDuration = 0.3f;

    [Header("Heath")]
    [SerializeField] int maxHP = 100;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    [SerializeField] float st_AtkJump_DownSpeedAfterJumpFinished = 10f;

    [Header("St_AtkHorSlash")]
    [Tooltip("NOTE: This should be shorter than the recovery animation dur.")]
    [SerializeField] float st_AtkHorSlash_RecoveryMotionInterpDur = 0.2f;
    [SerializeField] float st_AtkHorSlash_Impact_AngSpd = 800f;

    [Header("St_Dodge")]
    [SerializeField] float st_Dodge_YawAngSpd = 400f;

    [Header("St_Walk")]
    [SerializeField] float st_Walk_LinAcc = 100f;
    [SerializeField] int st_Walk_MaxLinSpd = 5;
    [SerializeField] float st_Walk_MaxAngSpd = 800f;

    // NOTE: This dumb conversion seems to be only way to have default inspector values since
    // NOTE C: C# 9 doesn't support default struct values. Otherwise I'd just have a serialized
    // NOTE C: struct directly in the scriptable object.
    public CapsuleCharacterData ToStruct() => new CapsuleCharacterData {
        gravitationalAcc = gravitationalAcc,
        inputBufferDuration = inputBufferDuration,
        maxHp = maxHP,
        curHp = maxHP,
        st_AtkJump_DownSpeedAfterJumpFinished = st_AtkJump_DownSpeedAfterJumpFinished,
        st_AtkHorSlash_RecoveryMotionInterpDur = st_AtkHorSlash_RecoveryMotionInterpDur,
        st_AtkHorSlash_Impact_AngSpd = st_AtkHorSlash_Impact_AngSpd,
        st_Dodge_YawAngSpd = st_Dodge_YawAngSpd,
        st_Walk_LinAcc = st_Walk_LinAcc,
        st_Walk_MaxLinSpd = st_Walk_MaxLinSpd,
        st_Walk_MaxAngSpd = st_Walk_MaxAngSpd
    };
}
