// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken!
using UnityEngine;

public class GameMgr : Singleton<GameMgr>{
    [SerializeField] PlrCtrl plrCtrl;
    [SerializeField] CpHandle plrPrefab;
    [SerializeField] Transform spawnPoint;

    override protected void Awake(){
        base.Awake();
        CpMgr.inst.Init();
        AiCtrlMgr.inst.Init();
    }
    private void Start() {
        CpSpawningUtils.SpawnPlrCpAtSpawnPt(plrPrefab, spawnPoint, plrCtrl, CamMgr.inst.cam);
        EnemyServer.inst.SpawnEnemies();
    }

    void FixedUpdate() {
        CpMgr.inst.FixedTick();
    }
    
    void Update() {
        // NOTE: Ai needs to be ticked before CpMgr for the ai ctrl input to work properly.
        AiCtrlMgr.inst.Tick();
        CpMgr.inst.Tick(Time.deltaTime);
    }

    void LateUpdate() {
        CpMgr.inst.LateTick();
        WldHpBarMgr.inst.LateTick();
    }
}
