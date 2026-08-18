using UnityEngine;

/// <summary>
/// Test object for shooting projectiles.
/// </summary>
public class Shooter : MonoBehaviour {
    [SerializeField] bool shoot = true;
    [SerializeField] AtkData atkData = new(10, KnockbackT.Weak, 0.5f);
    [SerializeField] float spd = 3;
    [SerializeField] float maxLifetime = 10;
    [SerializeField] float homingStr = 2;
    [SerializeField] float shootFreq = 1;
    [SerializeField] Transform plr;

    float timer = 0;
    HomingProjMovData projData;

    private void Awake() {
        projData = new(spd, maxLifetime, homingStr);
    }

    void Update(){
        if (!shoot)
            return;
        timer += Time.deltaTime;
        if(timer > shootFreq) {
            timer = 0;
            HomingProjMgr.inst.ShootProj(
                projData,
                atkData,
                transform.position,
                (plr.position - transform.position).normalized,
                plr);
        }
    }
}
