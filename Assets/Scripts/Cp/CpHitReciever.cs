using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
public class CpHitReciever : MonoBehaviour, IHitReceiver {
    [SerializeField] CpHandle cp;

    public PawnTeam GetTeam() 
        => CpMgr.GetAos(cp.Id).team;

    public bool IgnoreAllHits()
        => CpMgr.GetAos(cp.Id).invul;

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        //Debug.Log(
        //    $"HitData:\n" +
        //    $"  {nameof(hitData.effects.dmg)}: {hitData.effects.dmg}\n" +
        //    $"  {nameof(hitData.effects.knockbackT)}: {hitData.effects.knockbackT}\n" +
        //    $"  {nameof(hitData.effects.knockbackStr)}: {hitData.effects.knockbackStr}\n" +
        //    $"  {nameof(hitData.wldDir)}: {hitData.wldDir}"
        //);
        int cpId = this.cp.Id;
        ref Cp_AosData aos = ref CpMgr.GetAos(cpId);
        var classRefs = CpMgr.inst.classRefs[cpId];
        var cp = CpMgr.inst.cp[cpId];
        if (!aos.invul) {
            aos.hp_Cur -= hitData.effects.dmg;
            aos.hp_Cur = Mathf.Max(0, aos.hp_Cur);
            Dbg.Log($"New HP: {aos.hp_Cur}", this, aos.enableDbgMsgs);
            if (aos.hp_Cur == 0) {
                if (
                    CpMgr.inst.TrySwitchActSt(
                        () => classRefs.actSts.death.Enter(
                            CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Bwd)
                        ),
                        cpId
                    )
                )
                    return new(false, false);
            }
            aos.lastRecievedHitDir = hitData.wldDir;
            aos.lastKnockbackStr = hitData.effects.knockbackStr;
            //Debug.Log($"knockback str: {data.lastKnockbackStr[cpId]}.");
            switch (hitData.effects.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    Vector3 horHitDir = new Vector3(
                        aos.lastRecievedHitDir.x,
                        0,
                        aos.lastRecievedHitDir.z
                    );
                    // If you, for some reason, set the hit direction to Vector3.zero.
                    if (horHitDir.sqrMagnitude < 0.0001f)
                        horHitDir = Vector3.down;
                    else
                        horHitDir.Normalize();
                    AnimInfo knockbackAnim;
                    if (Vector3.Dot(horHitDir, cp.transform.forward) > 0)
                        knockbackAnim = CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Fwd);
                    else
                        knockbackAnim = CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Bwd);
                    CpMgr.inst.TrySwitchActSt(
                        () => classRefs.actSts.knockback.Enter(knockbackAnim),
                        cpId
                    );
                    break;
                case KnockbackT.Strong:
                    Debug.LogError("Strong knockback not implemented!", this);
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(aos.invul, false);
    }
}
