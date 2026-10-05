using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
public class CpHitReciever : MonoBehaviour, IHitReceiver {
    [SerializeField] CpHandle cp;

    public Team GetTeam => cp.so_cpData.team;
    /// <summary>
    /// NOTE This should be checked BEFORE calling <see cref="CpHitReciever.ReceiveHit"/>!
    /// </summary>
    bool IHitReceiver.IgnoreAllHits => cp.Data.ignoreHits;

    public HitResult ReceiveHit(HitData hitData) {
        //Debug.Log(
        //    $"HitData:\n" +
        //    $"  {nameof(hitData.effects.dmg)}: {hitData.effects.dmg}\n" +
        //    $"  {nameof(hitData.effects.knockbackT)}: {hitData.effects.knockbackT}\n" +
        //    $"  {nameof(hitData.effects.knockbackStr)}: {hitData.effects.knockbackStr}\n" +
        //    $"  {nameof(hitData.wldDir)}: {hitData.wldDir}"
        //);
        ref Cp_Data cpData = ref cp.Data;
        var classRefs = cpData.classRefs;
        var dmgDealt = Mathf.Min(hitData.hitEffects.dmg, cpData.hp_Cur);
        cpData.hp_Cur -= dmgDealt;
        cpData.action_DmgTaken?.Invoke(hitData.hitEffects.dmg);
        cpData.action_CurHpChanged?.Invoke(cpData.hp_Cur, cp.so_cpData.hp_Max);
        var safeHitNormal = hitData.normal.NrmSafe();
        switch (hitData.hitEffects.hitT) {
            case HitT.Blunt:
                SfxMgr.inst.PlaySfx(SfxId.DefaultLightSlam, hitData.hitPt);
                ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                break;
            case HitT.Cut:
                SfxMgr.inst.PlaySfx(SfxId.DefaultLightSlam, hitData.hitPt);
                ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                break;
            case HitT.Explosion:
                SfxMgr.inst.PlaySfx(SfxId.DefaultLightSlam, hitData.hitPt);
                ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                break;
            case HitT.Pierce:
                SfxMgr.inst.PlaySfx(SfxId.DefaultPierce, hitData.hitPt);
                ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                break;
            default:
                Debug.LogError($"Defaulted: {hitData.hitEffects.hitT}");
                break;
        }
        //Dbg.Log($"New HP: {aos.hp_Cur}", this, aos.enableDbgMsgs);
        if (cpData.hp_Cur == 0) {
            if (
                CpMgr.inst.TrySwitchActSt(
                    () => classRefs.actSts.death.Enter(
                        CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Bwd)
                    ),
                    cp.I
                )
            )
                return new(new IHitReceiver[] { this }, dmgDealt, false);
        }
        switch (hitData.hitDirMode) {
            case HitDirMode.FromHitSourceTrfToHitReciever:
                Vector3 dir = transform.position - hitData.srcTrf.position;
                cpData.lastRecievedHitDir = dir.NrmSafe();
                break;
            case HitDirMode.HitDealerMovDir:
                cpData.lastRecievedHitDir = hitData.hitDealerMovDir;
                break;
            default:
                Debug.LogError($"Defaulted: {hitData.hitDirMode}");
                break;
        }
        cpData.lastKnockbackStr = hitData.hitEffects.knockbackStr;
        //Debug.Log($"knockback str: {data.lastKnockbackStr[cpI]}.");
        switch (hitData.hitEffects.knockbackT) {
            case KnockbackT.None:
                break;
            case KnockbackT.Weak:
                Vector3 horHitDir = new Vector3(
                    cpData.lastRecievedHitDir.x,
                    0,
                    cpData.lastRecievedHitDir.z
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
        return new(new IHitReceiver[] { this }, dmgDealt, false);
    }
}
