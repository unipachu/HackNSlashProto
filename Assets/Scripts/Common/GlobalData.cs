using UnityEngine;

public class GlobalData : Singleton<GlobalData> {
    [Tooltip("What physics layers are considered for ground checks?")]
    public LayerMask groundMask = Physics.AllLayers;
    [Tooltip("How far is the ground allowed to be below a capsule pawn for the pawn to be considered grounded?")]
    public float isGroundedChkDist = 0.1f;
    [Tooltip("Duration which an input stays in the input buffer.")]
    public float inputBuffer_Dur = 0.3f;
    [Tooltip("Max downwards speed when falling for characters.")]
    public float maxFallSpd = 30;
    [Tooltip("In m/s^2. Should be around 9.81.")]
    public float gravitationalAcc = 10;
}
