using UnityEngine;

/// <summary>
/// Used as a memory managed reference to the capsule pawn's id. Also contains capsule pawn
/// initialization data.<br/>
/// NOTE: Set this to the root of the cp!
/// </summary>
// Rename to just Cp.
public class CpRegisterer : MonoBehaviour, LockOnTgt, IPawn{
    [Header("Scriptable Object Data")]
    public So_CpData so_cpData;
    
    [Header("Unity Comp Refs")]
    public Cp_UnityObjs unityObjs;

    public int Id { get; set; }
    // NOTE: This is a little cheating but we do not need to access the manager to access the lock on transform.
    public Transform Trf => unityObjs.lockOnTrf;
}
