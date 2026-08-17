using UnityEngine;

/// <summary>
/// Test object for shooting projectiles.
/// </summary>
public class Shooter : MonoBehaviour {
    [SerializeField] int dmg = 1;
    [SerializeField] float spd = 3;
    [SerializeField] float maxLifetime = 10;
    [SerializeField] float homingStr = 2;
    [SerializeField] float shootFreq = 1;
    [SerializeField] Transform plr;

    float timer = 0;
    HomingProjData projData;

    private void Awake() {
        projData = new(dmg, spd, maxLifetime, homingStr);
    }

    void Update(){
        timer += Time.deltaTime;
        if(timer > shootFreq) {
            timer = 0;
            HomingProjMgr.inst.ShootProj(
                projData,
                transform.position,
                (plr.position - transform.position).normalized,
                plr);
        }
    }
}
