using UnityEngine;

/// <summary>
/// Used as a memory managed handle to the entity id. Also contains capsule pawn initialization data.<br/>
/// NOTE: Set this to the root of the cp!
/// </summary>
public class CpHandle : MonoBehaviour, ILockOnTgt, IPawn{
    [Header("Scriptable Object Data")]
    public So_CpData so_cpData;
    
    [Header("Unity Comp Refs")]
    public Cp_UnityObjs unityObjs;

    public int Id { get; set; }
    public ILockOnTgt AsLockOnTgt => this;
    // NOTE: This is a little cheating but we do not need to access the manager to access the lock on transform.
    public Transform LockOnTrf => unityObjs.lockOnTrf;
}
