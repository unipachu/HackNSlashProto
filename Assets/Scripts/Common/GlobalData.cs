using UnityEngine;

/// <summary>
/// Values that are shared between all entities.
/// </summary>
public class GlobalData : Singleton<GlobalData> {
    [Tooltip("What physics layers are considered for ground checks?")]
    public LayerMask groundMask = Physics.AllLayers;
    [Tooltip("In m/s^2. Should be around 9.81.")]
    public float gravitationalAcc = 10;
    [Tooltip("Duration which an input stays in the input buffer.")]
    public float inputBuffer_Dur = 0.3f;
    [Tooltip("How far is the ground allowed to be below a capsule pawn for the pawn to be considered grounded?")]
    public float isGroundedChkDist = 0.1f;
    [Tooltip("Max downwards speed when falling for characters.")]
    public float maxFallSpd = 30;
}
