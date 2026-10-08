using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
public class CpHitReciever : MonoBehaviour, IHitReceiver {
    [SerializeField] InterfaceReference<ICp> cp;

    public Team GetTeam => cp.Value.So_CpCommonConfig.team;
    /// <summary>
    /// NOTE This should be checked BEFORE calling <see cref="CpHitReciever.ReceiveHit"/>!
    /// </summary>
    bool IHitReceiver.IgnoreAllHits => cp.Value.CommonData.ignoreHits;

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
        var dmgDealt = Mathf.Min(hitData.hitEffects.dmg, cp.Value.CommonData.hp_Cur);
        cp.Value.CommonData.hp_Cur -= dmgDealt;
        if(dmgDealt != 0) {
            cp.Value.CommonData.action_DmgTaken?.Invoke(hitData.hitEffects.dmg);
            cp.Value.CommonData.action_CurHpChanged?.Invoke(cp.Value.CommonData.hp_Cur, cp.Value.So_CpCommonConfig.hp_Max);
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
        //    $"New HP: {cp.ValueData.hp_Cur}",
        //    this,
        //    cp.ValueData.handle.so_cpData.enableDbgMsgs
        //);
        CalculateHitDir(cp.Value, hitData);
        cp.Value.CommonData.lastKnockbackStr = hitData.hitEffects.knockbackStr;
        //Dbg.Log(
        //    $"{nameof(hitData.hitEffects.knockbackStr)}: {hitData.hitEffects.knockbackStr}, "
        //        + $"{nameof(hitData.hitEffects.knockbackT)}: {hitData.hitEffects.knockbackT}.",
        //    this,
        //    cp.Value.so_cp.CommonData.enableDbgMsgs
        //);
        if (cp.Value.CommonData.hp_Cur == 0) {
            //Dbg.Log("Character hp went to 0!", this, cp.Value.So_CpCommonConfig.enableDbgMsgs);
            if (
                cp.Value.TryEnterDeathSt()
            )
                return new(new IHitReceiver[] { this }, dmgDealt, false);
        }
        if (!cp.Value.CommonData.hyperArmor) {
            switch (hitData.hitEffects.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    if (
                        cp.Value.So_CpCommonConfig.ignoredKnockback != KnockbackT.Weak
                            && cp.Value.So_CpCommonConfig.ignoredKnockback != KnockbackT.Strong
                    )
                        cp.Value.TrySetupNEnterKnockbackSt();
                    break;
                case KnockbackT.Strong:
                    if (cp.Value.So_CpCommonConfig.ignoredKnockback != KnockbackT.Strong)
                        cp.Value.TrySetupNEnterKnockbackSt();
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(new IHitReceiver[] { this }, dmgDealt, false);
    }

    static void CalculateHitDir(ICp cp, HitData hitData) {
        switch (hitData.hitDirMode) {
            case HitDirMode.FromHitSourceTrfToHitReciever:
                Vector3 dir = cp.Go.transform.position - hitData.srcTrf.position;
                cp.CommonData.lastRecievedHitDir = dir.NrmSafe();
                break;
            case HitDirMode.HitDealerMovDir:
                cp.CommonData.lastRecievedHitDir = hitData.hitDealerMovDir;
                break;
            default:
                Debug.LogError($"Defaulted: {hitData.hitDirMode}");
                break;
        }
    }


}
