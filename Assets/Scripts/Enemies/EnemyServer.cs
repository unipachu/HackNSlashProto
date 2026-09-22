using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Responsible for spawning enemies.
/// </summary>
public class EnemyServer : Singleton<EnemyServer> {
    [Serializable]
    public struct EnemyWave_EnemyEntry {
        public So_AiCpConfig enemyConfig;
        [Min(1)] public int amount;
    }

    [Serializable]
    public struct EnemyWave {
        public string name;

        [Header("Enemies")]
        public EnemyWave_EnemyEntry[] enemies;

        [Header("Next Wave Conditions")]
        [Min(0f)] public float timeUntilNextWave;
        [Tooltip("The next wave cannot start while the current enemy count is at or above this value. "
            + "Set to -1 to disable.")]
        public int blockNextWaveAtEnemyCount;
        [Tooltip("The next wave starts immediately when the current enemy count is at or below this value, "
            + "even if the timer has not expired. Set to -1 to disable.")]
        public int forceNextWaveAtEnemyCount;
    }

    [Header("Waves")]
    [SerializeField] EnemyWave[] waves;

    [Header("Spawn Points")]
    [SerializeField] EnemySpawnPt[] spawnPoints;

    int currentWaveIndex = -1;
    int currentEnemyCount;
    int nextSpawnPointIndex;
    float nextWaveTime;
    bool spawningWavesStarted;
    bool spawningWave;

    public int CurrentWaveIndex => currentWaveIndex;
    public int CurrentEnemyCount => currentEnemyCount;

    /// <summary>
    /// Call this when you want to start spawning waves.
    /// </summary>
    /// <param name="instaSpawnFirstWave">
    /// Should we skip spawning animations for the first wave (e.g. if we want the enemies to already be in
    /// the room when the game starts)?
    /// </param>
    public void StartSpawningWaves(bool instaSpawnFirstWave) {
        Debug.Assert(
            spawnPoints != null && spawnPoints.Length != 0, $"{nameof(EnemyServer)} has no spawn points.", this
        );
        Debug.Assert(waves != null && waves.Length != 0, $"{nameof(EnemyServer)} has no waves.", this);
        Debug.Assert(!spawningWavesStarted, $"{nameof(StartSpawningWaves)} was called more than once.", this);
        spawningWavesStarted = true;
        // Randomize first pawn pt
        nextSpawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        StartNextWave(instaSpawnFirstWave);
    }

    public void Tick(float dt, float now) {
        if (!spawningWavesStarted)
            return;
        if (spawningWave)
            return;
        if (!HasNextWave())
            return;
        EnemyWave currentWave = waves[currentWaveIndex];
        if (ShouldForceNextWave(currentWave)) {
            StartNextWave(false);
            return;
        }
        if (now < nextWaveTime)
            return;
        if (ShouldBlockNextWave(currentWave))
            return;
        StartNextWave(false);
    }

    void StartNextWave(bool instaSpawnWave) {
        currentWaveIndex++;
        EnemyWave wave = waves[currentWaveIndex];
        Debug.Log($"Starting enemy wave {currentWaveIndex}: {wave.name}");
        StartCoroutine(SpawnWaveRoutine(wave, instaSpawnWave));
    }

    IEnumerator SpawnWaveRoutine(EnemyWave wave, bool instaSpawnWave) {
        spawningWave = true;
        for (int i = 0; i < wave.enemies.Length; i++) {
            EnemyWave_EnemyEntry entry = wave.enemies[i];
            if (entry.enemyConfig == null) {
                Debug.LogError($"Wave {currentWaveIndex} contains an enemy entry with no enemy config.");
                continue;
            }
            for (int j = 0; j < entry.amount; j++) {
                EnemySpawnPt spawnPt = null;
                while (spawnPt == null) {
                    spawnPt = TryGetNextSpawnPt(instaSpawnWave);
                    if (spawnPt == null)
                        yield return null; // No free spawn point available, wait.
                }
                AiCtrlHandle aiCtrl = CpFactory.SpawnAiCpAtSpawnPt(entry.enemyConfig, spawnPt.transform);
                aiCtrl.Data.cp.GetData().action_died += OnEnemyDied;
                currentEnemyCount++;
            }
        }
        spawningWave = false;
        nextWaveTime = Time.time + wave.timeUntilNextWave;
    }

    /// <summary>
    /// Returns next spawn pt, null if none are free.
    /// </summary>
    /// <param name="forceGetSpawnPt">
    /// Should this forcibly return next spawn point even if it otherwise was not available for spawning?
    /// </param>
    EnemySpawnPt TryGetNextSpawnPt(bool forceGetSpawnPt) {
        for (int i = 0; i < spawnPoints.Length; i++) {
            EnemySpawnPt spawnPoint = spawnPoints[nextSpawnPointIndex];
            nextSpawnPointIndex++;
            if (nextSpawnPointIndex >= spawnPoints.Length)
                nextSpawnPointIndex = 0;
            if (!forceGetSpawnPt && !spawnPoint.TryBeginSpawning())
                continue;
            return spawnPoint;
        }
        return null;
    }

    bool HasNextWave()
        => currentWaveIndex + 1 < waves.Length;
    
    bool ShouldBlockNextWave(EnemyWave wave) {
        if (wave.blockNextWaveAtEnemyCount < 0)
            return false;
        return currentEnemyCount >= wave.blockNextWaveAtEnemyCount;
    }

    bool ShouldForceNextWave(EnemyWave wave) {
        if (wave.forceNextWaveAtEnemyCount < 0)
            return false;
        return currentEnemyCount <= wave.forceNextWaveAtEnemyCount;
    }

    public void OnEnemyDied(CpHandle cp) {
        cp.GetData().action_died -= OnEnemyDied;
        currentEnemyCount--;
        if (currentEnemyCount == 0 && spawningWave == false && !HasNextWave())
            Debug.Log("Player completed all enemy waves!!!", this);
        Debug.Assert(currentEnemyCount >= 0, $"{nameof(currentEnemyCount)} went below zero.", this);
    }
}