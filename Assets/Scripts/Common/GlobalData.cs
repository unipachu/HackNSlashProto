using UnityEngine;

/// <summary>
/// Values that are shared between all entities.
/// </summary>
public class GlobalData : Singleton<GlobalData> {
    [field: SerializeField]
    [field: Tooltip("AnimEventPlr currently converts aniamtion time by using this for all animations.")]
    public int animSampleRate { get; private set; } = 30;
    [field: SerializeField]
    [field: Tooltip("Duration of the visual effect that warns the player of enemy spawning there.")]
    public float enemySpawnAnimDur { get; private set; } = 2f;
    [field: SerializeField]
    [field: Tooltip("In m/s^2. Should be around 9.81.")]
    public float gravitationalAcc { get; private set; } = 10f;
    [field: SerializeField]
    [field: Tooltip("What physics layers are considered for ground checks?")]
    public LayerMask groundMask { get; private set; } = Physics.AllLayers;
    [field: SerializeField]
    [field: Tooltip("Duration which an input stays in the input buffer.")]
    public float inputBuffer_Dur { get; private set; } = 0.3f;
    [field: SerializeField]
    [field: Tooltip("How far is the ground allowed to be below a capsule pawn for the pawn to be "
        + "considered grounded?")]
    public float isGroundedChkDist { get; private set; } = 0.1f;
    [field: SerializeField]
    [field: Tooltip("Max downwards speed when falling for characters.")]
    public float maxFallSpd { get; private set; } = 30f;
    [field: SerializeField]
    [field: Tooltip("Window (in seconds) after an attack during which another attack is"
        + "considered 'successive'.")]
    public float successiveAtkWindow { get; private set; } = 1.5f;
}
