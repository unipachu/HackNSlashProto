// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken!
using UnityEngine;

public class GameMgr : Singleton<GameMgr>{
    [SerializeField] PlrMgr plrMgr;
    // TODO: PlrMgr should be responsible for spawning the player.
    [SerializeField] CpHandle plrPrefab;
    [SerializeField] Transform spawnPoint;

    override protected void Awake(){
        base.Awake();
        CpMgr.inst.Init();
        AiCtrlMgr.inst.Init();
    }
    private void Start() {
        CpFactory.SpawnPlrCpAtSpawnPt(plrPrefab, spawnPoint, plrMgr, CamMgr.inst.cam);
        EnemyServer.inst.StartSpawningWaves(true);
    }

    void FixedUpdate() {
        CpMgr.inst.FixedTick();
    }
    
    void Update() {
        // NOTE: Ai needs to be ticked before CpMgr for the ai ctrl input to work properly.
        AiCtrlMgr.inst.Tick();
        CpMgr.inst.Tick(Time.deltaTime);
        EnemyServer.inst.Tick(Time.deltaTime, Time.time);
    }

    void LateUpdate() {
        CpMgr.inst.LateTick();
        WldHpBarMgr.inst.LateTick();
    }
}
