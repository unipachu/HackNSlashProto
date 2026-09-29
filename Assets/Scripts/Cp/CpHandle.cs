using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Used as a memory managed handle to the entity id. Also contains capsule pawn initialization data.<br/>
/// NOTE: Set this to the root of the cp!
/// </summary>
public class CpHandle : MonoBehaviour, ILockOnTargetable, IPawn, IFollowTgt {
    [Header("Scriptable Object Data")]
    public So_CpData so_cpData;
    
    [Header("Unity Obj Refs")]
    public Animator anim;
    public CpAnimEventHandler animEventHandler;
    public CharacterController cc;
    public CpHitReciever hitReciever;
    public NavMeshAgent navMeshAgent;
    public Transform rHand;
    public Transform lockOnTrf;
    public Transform wldHpBarPos;

    /// <summary>
    /// Index to the corresponding entity data <see cref="CpMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public ref Cp_Data Data => ref CpMgr.inst.aos[I];
    public Transform LockOnTrf => lockOnTrf;
    public Transform TrfToFollow => transform;

    public bool IsOnNavMesh()
        => CpUtils.IsOnNavMesh(I);
}
