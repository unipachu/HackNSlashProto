using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Used as a memory managed handle to the entity index. Also contains capsule pawn initialization data.<br/>
/// NOTE: Set this to the root of the capsule pawn!
/// </summary>
public class CpFlyingHeadHandle : MonoBehaviour, ILockOnTargetable, ICp, IFollowTgt {
    [Header("Scriptable Object Data")]
    public string dataId;

    [Header("Unity Obj Refs")]
    public Animator anim;
    public CharacterController cc;
    public HitDealer_SphereCast hitDealer;
    public CpHitReciever hitReciever;
    public NavMeshAgent navMeshAgent;
    public Transform lockOnTrf;
    public Transform wldHpBarPos;

    /// <summary>
    /// Index to the corresponding entity data in <see cref="CpFlyingHeadMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public Animator Anim => anim;
    public CharacterController Cc => cc;
    public ref Cp_CommonData CommonData => ref CpFlyingHeadMgr.inst.commonData[I];
    public DbRow_CpCommonConfig CpCommonConfig => Db.inst.db.GetCpCommonConfig(dataId);
    public ref CpFlyingHead_Data FlyingHeadData => ref CpFlyingHeadMgr.inst.flyingHeadData[I];
    public GameObject Go => gameObject;
    public Transform LockOnTrf => lockOnTrf;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Transform TrfToFollow => transform;
    public Transform WldHpBarPos => wldHpBarPos;

    public bool IsOnNavMesh()
        => CpUtils.IsOnNavMesh(ref CommonData);

    public void OnHitSomething(HashSet<HitResult> hits) {
        CommonData.action_HitSomething?.Invoke(hits);
    }

    public bool TryEnterDeathSt() {
        if (CpUtils.TrySwitchActSt(
            () => FlyingHeadData.actSts.death.Enter(CpFlyingHeadUtils.FindKnockbackAnim(CommonData)),
                ref CommonData
            )
        )
            return true;
        return false;
    }

    public bool TrySetupNEnterKnockbackSt()
        => CpUtils.TrySwitchActSt(
            () => FlyingHeadData.actSts.knockback.Enter(CpFlyingHeadUtils.FindKnockbackAnim(CommonData)),
            ref CommonData
        );
}
