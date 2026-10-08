using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Used as a memory managed handle to the entity index. Also contains capsule pawn initialization data.<br/>
/// NOTE: Set this to the root of the capsule pawn!
/// </summary>
public class CpFlyingHeadHandle : MonoBehaviour, ILockOnTargetable, ICp, IFollowTgt {
    [Header("Scriptable Object Data")]
    public So_CpCommonConfig so_cpData;
    
    [Header("Unity Obj Refs")]
    public Animator anim;
    public CpAnimEventHandler animEventHandler;
    public CharacterController cc;
    public CpHitReciever hitReciever;
    public NavMeshAgent navMeshAgent;
    public Transform lockOnTrf;
    public Transform wldHpBarPos;

    /// <summary>
    /// Index to the corresponding entity data in <see cref="CpFlyingHeadMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public Animator Anim => throw new System.NotImplementedException();
    public CpFlyingHeadAnimEventHandler AnimEventHandler => throw new System.NotImplementedException();
    public CharacterController Cc => throw new System.NotImplementedException();
    public So_CpCommonConfig So_CpCommonConfig => throw new System.NotImplementedException();
    public ref Cp_CommonData CommonData => throw new System.NotImplementedException();
    public GameObject Go => throw new System.NotImplementedException();
    public Transform LockOnTrf => lockOnTrf;
    public NavMeshAgent NavMeshAgent => throw new System.NotImplementedException();
    public Transform TrfToFollow => transform;
    public Transform WldHpBarPos => throw new System.NotImplementedException();

    public bool IsOnNavMesh()
        => CpUtils.IsOnNavMesh(ref CommonData);

    public void OnHitSomething(HashSet<HitResult> hits) {
        CommonData.action_HitSomething?.Invoke(hits);
    }

    public bool TryEnterDeathSt() {
        throw new System.NotImplementedException();
    }

    public bool TrySetupNEnterKnockbackSt() {
        // TODO: Check how the humanoid cp does this.
        throw new System.NotImplementedException();
    }
}
