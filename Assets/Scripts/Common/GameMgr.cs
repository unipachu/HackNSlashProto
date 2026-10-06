// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken!
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : Singleton<GameMgr> {
    [SerializeField] PlrCtrl plrMgr;
    [SerializeField] CpHandle plrPrefab;
    [SerializeField] Transform spawnPoint;

    [Header("Game Over")]
    [SerializeField] CanvasGroup gameOverScreen;
    [SerializeField] float gameOverFadeDur = 0.5f;
    [Tooltip("Game over duration in unscaled time.")]
    [SerializeField] float gameOverVisibleDur = 3f;

    [HideInInspector] CpHandle plrCp;

    bool gameOver;
    float gameOverTimer;

    // -------------------------------------------------------------------------
    // Unity Callbacks
    // -------------------------------------------------------------------------

    override protected void Awake() {
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

    void Start() {
        Debug.Assert(gameOverScreen != null, "Game over screen was null!");
        gameOverScreen.gameObject.SetActive(false);
        plrCp = CpFactory.SpawnPlrCpAtSpawnPt(plrPrefab, spawnPoint, plrMgr, CamMgr.inst.cam);
        plrCp.Data.action_Died += OnPlrDied;
        EnemyWaveMgr.inst.StartSpawningWaves(true);
    }

    void FixedUpdate() {
        CpMgr.inst.FixedTick();
    }

    void Update() {
        if (gameOver)
            UpdateGameOver();
        float dt = Time.deltaTime;
        // NOTE: Ai needs to be ticked before CpMgr for the ai ctrl input to work properly.
        AiCtrlMgr.inst.Tick(dt);
        CpMgr.inst.Tick(dt);
        EnemyWaveMgr.inst.Tick(dt, Time.time);
    }

    void LateUpdate() {
        CpMgr.inst.LateTick(Time.deltaTime);
        CamMgr.inst.LateTick();
        WldHpBarMgr.inst.LateTick();
        PlrMgr.inst.LateTick();
    }

    // -------------------------------------------------------------------------
    // Other Methods
    // -------------------------------------------------------------------------

    void UpdateGameOver() {
        gameOverTimer += Time.unscaledDeltaTime;
        float fadeNrm = gameOverFadeDur > 0f
            ? Mathf.Clamp01(gameOverTimer / gameOverFadeDur)
            : 1f;
        gameOverScreen.alpha = fadeNrm;
        if (gameOverTimer >= gameOverFadeDur + gameOverVisibleDur)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    static void ApplySettings() {
        Application.targetFrameRate = GameSettings.inst.targetFrameRate;
        QualitySettings.vSyncCount = GameSettings.inst.vSyncCount;
    }

    /// <summary>
    /// Game over logic.
    /// </summary>
    void OnPlrDied(CpHandle cp) {
        plrCp.Data.action_Died -= OnPlrDied;
        CamMgr.inst.movByInputAllowed = false;
        TimeMgr.SetSlowMotion(true);
        gameOver = true;
        gameOverTimer = 0f;
        gameOverScreen.alpha = 0f;
        gameOverScreen.gameObject.SetActive(true);
    }
}