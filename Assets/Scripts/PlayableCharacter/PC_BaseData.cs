using UnityEngine;

/// <summary>
/// Playable character static base data.
/// NOTE: State specific data starts with "St_[state name]_".
/// </summary>
[CreateAssetMenu(fileName = "PCData", menuName = "Character Data/PC Data")]
public class PC_BaseData : ScriptableObject
{
    [field: Header("General Movement Settings")]
    [field: Tooltip("In m/s^2. Should be around 9.81.")]
    [field: SerializeField] public float GravitationalAcc { get; private set; } = 15f;
    [field: Tooltip("How long should inputs stay in the buffer?")]
    [field: SerializeField] public float InputBufferDuration { get; private set; } = 0.4f;

    [field: Header("St_AtkJump")]
    [field: Tooltip("In m/s. Should be positive.")]
    [field: SerializeField] public float St_AtkJump_DownSpeedAfterJumpFinished { get; private set; } = 10f;

    // TODO: Do you even use the movement settings for the attacks? Also these are the input based speeds, not anim root motion speeds.
    [field: Header("St_AtkHorSlash")]
    //[field: SerializeField] public float St_AtkHorSlash1_LinAcc { get; private set; } = 50f;
    //[field: SerializeField] public int St_AtkHorSlash1_MaxLinSpd { get; private set; } = 2;
    //[field: SerializeField] public float St_AtkHorSlash1_MaxAngSpd { get; private set; } = 200f;
    [field: Tooltip("NOTE: This should be shorter than the recovery animation dur.")]
    [field: SerializeField] public float St_AtkHorSlash_RecoveryMotionInterpDur { get; private set; } = 0.2f;
    [field: SerializeField] public float St_AtkHorSlash_Impact_AngSpd { get; private set; } = 1000f;


    [field: Header("St_Walk")]
    [field: SerializeField] public float St_Walk_LinAcc { get; private set; } = 100f;
    [field: SerializeField] public int St_Walk_MaxLinSpd { get; private set; } = 5;
    [field: SerializeField] public float St_Walk_MaxAngSpd { get; private set; } = 800f;
}
