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
        //Dbg.Log(
        //    $"HitData:\n"
        //        + $"  {nameof(hitData.hitEffects.dmg)}: {hitData.hitEffects.dmg}\n"
        //        + $"  {nameof(hitData.hitEffects.knockbackT)}: {hitData.hitEffects.knockbackT}\n"
        //        + $"  {nameof(hitData.hitEffects.knockbackStr)}: {hitData.hitEffects.knockbackStr}\n"
        //        + $"  {nameof(hitData.hitDealerMovDir)}: {hitData.hitDealerMovDir}",
        //    this,
        //    cp.so_cpData.enableDbgMsgs
        //);
        ref Cp_Data cpData = ref cp.Data;
        var classRefs = cpData.classRefs;
        var dmgDealt = Mathf.Min(hitData.hitEffects.dmg, cpData.hp_Cur);
        cpData.hp_Cur -= dmgDealt;
        if(dmgDealt != 0) {
            cpData.action_DmgTaken?.Invoke(hitData.hitEffects.dmg);
            cpData.action_CurHpChanged?.Invoke(cpData.hp_Cur, cp.so_cpData.hp_Max);
        }
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
        //Dbg.Log(
        //    $"New HP: {cpData.hp_Cur}",
        //    this,
        //    cpData.handle.so_cpData.enableDbgMsgs
        //);
        CalculateHitDir(cp, hitData);
        cpData.lastKnockbackStr = hitData.hitEffects.knockbackStr;
        //Dbg.Log(
        //    $"{nameof(hitData.hitEffects.knockbackStr)}: {hitData.hitEffects.knockbackStr}, "
        //        + $"{nameof(hitData.hitEffects.knockbackT)}: {hitData.hitEffects.knockbackT}.",
        //    this,
        //    cp.so_cpData.enableDbgMsgs
        //);
        if (cpData.hp_Cur == 0) {
            if (
                CpMgr.inst.TrySwitchActSt(
                    () => classRefs.actSts.death.Enter(
                        FindKnockbackAnim(cp)
                    ),
                    cp.I
                )
            )
                return new(new IHitReceiver[] { this }, dmgDealt, false);
        }
        if (!cpData.hyperArmor) {
            switch (hitData.hitEffects.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    if (
                        cp.so_cpData.ignoredKnockback != KnockbackT.Weak
                            && cp.so_cpData.ignoredKnockback != KnockbackT.Strong
                    )
                        SetupNEnterKnockbackSt(cp);
                    break;
                case KnockbackT.Strong:
                    if (cp.so_cpData.ignoredKnockback != KnockbackT.Strong)
                        SetupNEnterKnockbackSt(cp);
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(new IHitReceiver[] { this }, dmgDealt, false);
    }

    static void CalculateHitDir(CpHandle cp, HitData hitData) {
        ref var cpData = ref cp.Data;
        switch (hitData.hitDirMode) {
            case HitDirMode.FromHitSourceTrfToHitReciever:
                Vector3 dir = cp.transform.position - hitData.srcTrf.position;
                cpData.lastRecievedHitDir = dir.NrmSafe();
                break;
            case HitDirMode.HitDealerMovDir:
                cpData.lastRecievedHitDir = hitData.hitDealerMovDir;
                break;
            default:
                Debug.LogError($"Defaulted: {hitData.hitDirMode}");
                break;
        }
    }

    static void SetupNEnterKnockbackSt(CpHandle cp) {
        CpMgr.inst.TrySwitchActSt(
            () => cp.Data.classRefs.actSts.knockback.Enter(FindKnockbackAnim(cp)),
            cp.I
        );
    }

    static AnimInfo FindKnockbackAnim(CpHandle cp) {
        var cpData = cp.Data;
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
        if (Vector3.Dot(horHitDir, cp.transform.forward) > 0)
            // TODO MAYBE: Create different animation for "strong knockback".
            return CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Fwd);
        return CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Bwd);
    }
}
