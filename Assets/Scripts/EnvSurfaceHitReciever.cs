using UnityEngine;

public class EnvSurfaceHitReciever : MonoBehaviour, IHitReceiver{
    [SerializeField] SurfaceT surfaceT;

    public Team GetTeam => Team.EnemyToAll;
    bool IHitReceiver.IgnoreAllHits => false;

    public HitResult ReceiveHit(HitData hitData) {
        //Debug.Log(
        //    $"HitData:\n" +
        //    $"  {nameof(hitData.effects.dmg)}: {hitData.effects.dmg}\n" +
        //    $"  {nameof(hitData.effects.knockbackT)}: {hitData.effects.knockbackT}\n" +
        //    $"  {nameof(hitData.effects.knockbackStr)}: {hitData.effects.knockbackStr}\n" +
        //    $"  {nameof(hitData.wldDir)}: {hitData.wldDir}"
        //);
        SpawnSurfaceHitEffects(surfaceT, hitData);
        return new(new IHitReceiver[] { this }, 0, false);
    }

    static void SpawnSurfaceHitEffects(SurfaceT surfaceT, HitData hitData) {
        // If hit failed to find a normal, we use up direction.
        var safeHitNormal = hitData.normal.NrmSafe();
        switch (surfaceT) {
            case SurfaceT.Default:
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
                break;
            case SurfaceT.Metal:
                switch (hitData.hitEffects.hitT) {
                    case HitT.Blunt:
                        SfxMgr.inst.PlaySfx(SfxId.MetalLightSlam, hitData.hitPt);
                        ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                        break;
                    case HitT.Cut:
                        SfxMgr.inst.PlaySfx(SfxId.MetalLightSlam, hitData.hitPt);
                        ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                        break;
                    case HitT.Explosion:
                        SfxMgr.inst.PlaySfx(SfxId.MetalLightSlam, hitData.hitPt);
                        ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                        break;
                    case HitT.Pierce:
                        SfxMgr.inst.PlaySfx(SfxId.MetalPierce, hitData.hitPt);
                        ParticleFactory.inst.PlaySparks(hitData.hitPt, safeHitNormal);
                        break;
                    default:
                        Debug.LogError($"Defaulted: {hitData.hitEffects.hitT}");
                        break;
                }
                break;
            default:
                Debug.LogError($"Defaulted: {surfaceT}");
                break;
        }
    }
}