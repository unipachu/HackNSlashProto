using UnityEngine;

/// <summary>
/// Test object for shooting projectiles.
/// </summary>
public class TestShooter : MonoBehaviour {
    [SerializeField] bool shoot = true;
    [SerializeField] HitEffects hitEffects = new(10, KnockbackT.Weak, 0.5f);
    [SerializeField] PawnTeam team = PawnTeam.EnemyToAll;
    [SerializeField] float spd = 3;
    [SerializeField] float maxLifetime = 10;
    [SerializeField] float homingStr = 2;
    [SerializeField] float shootInterval = 1;
    [SerializeField] Transform plr;

    float timer = 0;
    HomingProjData projData;

    private void Awake() {
        projData = new(spd, maxLifetime, homingStr);
    }

    void Update(){
        if (!shoot)
            return;
        timer += Time.deltaTime;
        if(timer > shootInterval) {
            timer = 0;
            HomingProjMgr.inst.ShootProj(
                projData,
                // TODO: Build hit data in the projectile (since it can change direction).
                new HitData(
                    hitEffects,
                    team,
                    HitDirMode.WldDir,
                    null,
                    (plr.position - transform.position).normalized
                ),
                null,
                transform.position,
                (plr.position - transform.position).normalized,
                plr);
        }
    }
}
