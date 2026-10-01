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
        return new(new IHitReceiver[] { this }, false, false);
    }

    // TODO: Move to utils
    static void SpawnSurfaceHitEffects(SurfaceT surfaceT, HitData hitData) {
        // TODO: Read hit type from hit data i.e. slash, slam, piecer, or explosion
        switch (surfaceT) {
            case SurfaceT.Default:
                SfxMgr.inst.PlaySfx(SfxId.DefaultLightSlam, hitData.hitPt);
                break;
            case SurfaceT.Metal:
                break;
            default:
                Debug.LogError($"Defaulted: {surfaceT}");
                break;
        }
    }
}