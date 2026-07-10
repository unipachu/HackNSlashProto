using UnityEngine;

/// <summary>
/// Playable character static base data.
/// NOTE: State specific data starts with "St_[state name]_".
/// </summary>
[CreateAssetMenu(fileName = "PCData", menuName = "Character Data/PC Data")]
public class PC_BaseData : ScriptableObject
{
    [Header("General Movement Settings")]
    [field: Tooltip("In m/s^2. Should be around 9.81.")]
    [field: SerializeField] public float GravitationalAcc { get; private set; } = 15f;

    [Header("St_AttackJump")]
    [field: Tooltip("In m/s. Should be positive.")]
    [field: SerializeField] public float St_AtkJump_DownSpeedAfterJumpFinished { get; private set; } = 10f;


    [Header("St_Walk")]
    [field: SerializeField] public float St_Walk_LinAcc { get; private set; } = 100f;
    [field: SerializeField] public int St_Walk_MaxLinSpd { get; private set; } = 5;
    [field: SerializeField] public float St_Walk_MaxAngSpd { get; private set; } = 800f;
}
