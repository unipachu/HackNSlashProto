using UnityEngine;

/// <summary>
/// Test object for shooting projectiles.
/// </summary>
public class TestShooter : MonoBehaviour {
    [SerializeField] bool shoot = true;
    [SerializeField] HitEffects hitEffects = new(10, HitT.Blunt, KnockbackT.Weak, 0.5f);
    [SerializeField] Team team = Team.EnemyToAll;
    [SerializeField] float spd = 3;
    [SerializeField] float maxLifetime = 10;
    [SerializeField] float homingStr = 2;
    [SerializeField] float shootInterval = 1;
    [SerializeField] Transform plr;

    float timer = 0;
    HomingProjData homingProjData;

    private void Awake() {
        homingProjData = new(spd, maxLifetime, homingStr);
    }

    void Update(){
        if (!shoot)
            return;
        timer += Time.deltaTime;
        if(timer > shootInterval) {
            timer = 0;
            HomingProjMgr.inst.ShootProj(
                null,
                HitDirMode.HitDealerMovDir,
                hitEffects,
                homingProjData,
                ProjT.SphereProj,
                null,
                null,
                team,
                transform.position,
                (plr.position - transform.position).normalized
            );
        }
    }
}
