using UnityEngine;

/// <summary>
/// Used as a memory managed handle to the entity id. Also contains capsule pawn initialization data.<br/>
/// NOTE: Set this to the root of the cp!
/// </summary>
public class CpHandle : MonoBehaviour, ILockOnTargetable, IPawn, IFollowTgt {
    [Header("Scriptable Object Data")]
    public So_CpData so_cpData;
    
    [Header("Unity Comp Refs")]
    public Cp_UnityObjs unityObjs;

    /// <summary>
    /// Index to the corresponding entity data <see cref="CpMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public ref Cp_Data Data => ref CpMgr.inst.aos[I];
    // NOTE: This is a little cheating but we do not need to access the manager to access the lock on transform.
    public Transform LockOnTrf => unityObjs.lockOnTrf;
    public Transform TrfToFollow => transform;

    // TODO: Make this a property.

    public bool IsOnNavMesh()
        => CpUtils.IsOnNavMesh(I);
}
