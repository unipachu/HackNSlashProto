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
    ///  Rename to "I" since this object is kind of the id and this int is index of the entity.
    /// </summary>
    public int Id { get; set; }

    // NOTE: This is a little cheating but we do not need to access the manager to access the lock on transform.
    public Transform LockOnTrf => unityObjs.lockOnTrf;
    public Transform TrfToFollow => transform;

    public ref Cp_AosData GetData()
        => ref CpMgr.GetData(Id);

    public bool IsOnNavMesh()
        => CpUtils.IsOnNavMesh(Id);
}
