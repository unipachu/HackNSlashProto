using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
public class CpHitReciever : MonoBehaviour, IHitReceiver {
    [SerializeField] CpHandle cp;

    public PawnTeam GetTeam() 
        => cp.Data.team;

    public bool IgnoreAllHits()
        => cp.Data.ignoreHits;

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        //Debug.Log(
        //    $"HitData:\n" +
        //    $"  {nameof(hitData.effects.dmg)}: {hitData.effects.dmg}\n" +
        //    $"  {nameof(hitData.effects.knockbackT)}: {hitData.effects.knockbackT}\n" +
        //    $"  {nameof(hitData.effects.knockbackStr)}: {hitData.effects.knockbackStr}\n" +
        //    $"  {nameof(hitData.wldDir)}: {hitData.wldDir}"
        //);
        ref Cp_Data aos = ref cp.Data;
        var classRefs = aos.classRefs;
        if (!aos.ignoreHits) {
            aos.hp_Cur -= hitData.effects.dmg;
            aos.hp_Cur = Mathf.Max(0, aos.hp_Cur);
            aos.action_dmgTaken?.Invoke(hitData.effects.dmg);
            aos.action_curHpChanged?.Invoke(aos.hp_Cur, aos.hp_Max);
            //Dbg.Log($"New HP: {aos.hp_Cur}", this, aos.enableDbgMsgs);
            if (aos.hp_Cur == 0) {
                if (
                    CpMgr.inst.TrySwitchActSt(
                        () => classRefs.actSts.death.Enter(
                            CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Bwd)
                        ),
                        cp.I
                    )
                )
                    return new(false, false);
            }
            aos.lastRecievedHitDir = hitData.wldDir;
            aos.lastKnockbackStr = hitData.effects.knockbackStr;
            //Debug.Log($"knockback str: {data.lastKnockbackStr[cpI]}.");
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
                        cp.I
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
        return new(aos.ignoreHits, false);
    }
}
