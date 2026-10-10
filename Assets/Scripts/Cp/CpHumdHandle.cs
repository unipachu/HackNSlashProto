using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Used as a memory managed handle to the entity index. Also contains capsule pawn initialization data.<br/>
/// NOTE: Set this to the root of the capsule pawn!
/// </summary>
public class CpHumdHandle : MonoBehaviour, ILockOnTargetable, ICp, IFollowTgt {
    [Header("Scriptable Object Data")]
    public string dataId;
    public So_CpHumdConfig so_cpHumdConfig;
    
    [Header("Unity Obj Refs")]
    public Animator anim;
    public CharacterController cc;
    public CpHitReciever hitReciever;
    public NavMeshAgent navMeshAgent;
    public Transform rHand;
    public Transform lockOnTrf;
    public Transform wldHpBarPos;

    /// <summary>
    /// Index to the corresponding entity data in <see cref="CpHumdMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public Animator Anim => anim;
    public CharacterController Cc => cc;
    public ref Cp_CommonData CommonData => ref CpHumdMgr.inst.commonData[I];
    public DbRow_CpCommonConfig CpCommonConfig => Db.inst.db.GetCpCommonConfig(dataId);
    public GameObject Go => gameObject;
    public ref CpHumd_Data HumdData => ref CpHumdMgr.inst.humdData[I];
    public Transform LockOnTrf => lockOnTrf;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Transform TrfToFollow => transform;
    public Transform WldHpBarPos => wldHpBarPos;

    public bool IsOnNavMesh()
        => CpUtils.IsOnNavMesh(ref CommonData);

    public void OnHitSomething(HashSet<HitResult> hits) {
        CommonData.action_HitSomething?.Invoke(hits);
    }

    public bool TrySetupNEnterKnockbackSt()
        => CpUtils.TrySwitchActSt(
            () => HumdData.classRefs.actSts.knockback.Enter(CpHumdUtils.FindKnockbackAnim(CommonData)),
            ref CommonData
        );

    public bool TryEnterDeathSt() {
        if (CpUtils.TrySwitchActSt(
            () => HumdData.classRefs.actSts.death.Enter(CpHumdUtils.FindKnockbackAnim(CommonData)),
                ref CommonData
            )
        )
            return true;
        return false;
    }
}
