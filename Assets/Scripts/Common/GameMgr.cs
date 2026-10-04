// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken!
using UnityEngine;

public class GameMgr : Singleton<GameMgr>{
    [SerializeField] PlrMgr plrMgr;
    // TODO: PlrMgr should be responsible for spawning the player.
    [SerializeField] CpHandle plrPrefab;
    [SerializeField] Transform spawnPoint;

    [HideInInspector] CpHandle plrCp;

    override protected void Awake(){
        base.Awake();
        ApplySettings();
        TimeMgr.UpdateTimeScl();
        WldHpBarMgr.inst.Init();
        CpMgr.inst.Init();
        AiCtrlMgr.inst.Init();
        ParticleFactory.inst.Init();
        SfxMgr.inst.Init();
        CamMgr.inst.Init();
    }
    private void Start() {
        plrCp = CpFactory.SpawnPlrCpAtSpawnPt(plrPrefab, spawnPoint, plrMgr, CamMgr.inst.cam);
        plrCp.Data.action_markedForPendingUnregister += OnPlrMarkedForPendingUnregister;
        EnemyWaveMgr.inst.StartSpawningWaves(true);
    }

    void FixedUpdate() {
        CpMgr.inst.FixedTick();
    }
    
    void Update() {
        float dt = Time.deltaTime;
        // NOTE: Ai needs to be ticked before CpMgr for the ai ctrl input to work properly.
        AiCtrlMgr.inst.Tick(dt);
        CpMgr.inst.Tick(dt);
        EnemyWaveMgr.inst.Tick(dt, Time.time);
    }

    void LateUpdate() {
        CpMgr.inst.LateTick();
        CamMgr.inst.LateTick();
        WldHpBarMgr.inst.LateTick();
        HudMgr.inst.LateTick();
    }

    static void ApplySettings() {
        Application.targetFrameRate = GameSettings.inst.targetFrameRate;
        QualitySettings.vSyncCount = GameSettings.inst.vSyncCount;
    }

    /// <summary>
    /// Game over logic.
    /// </summary>
    void OnPlrMarkedForPendingUnregister() {
        plrCp.Data.action_markedForPendingUnregister -= OnPlrMarkedForPendingUnregister;
        CamMgr.inst.movByInputAllowed = false;
    }
}
