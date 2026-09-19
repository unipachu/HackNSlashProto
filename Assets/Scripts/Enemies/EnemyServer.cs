using UnityEngine;

// TODO: Enemy wave manager basically. 
public class EnemyServer : Singleton<EnemyServer>{
    [SerializeField] So_AiCpConfig hammerEnemyConfig;
    [SerializeField] So_AiCpConfig pistolEnemyConfig;
    [SerializeField] Transform[] spawnPoints;

    public void SpawnEnemies() {
        for (int i = 0; i < spawnPoints.Length; i++) {
            //Debug.Log($"Spawnin enemy to spawn point: {i}");
            if (i % 2 == 0) 
                CpSpawningUtils.SpawnAiCpAtSpawnPt(
                    hammerEnemyConfig,
                    spawnPoints[i]
                );
            else
                CpSpawningUtils.SpawnAiCpAtSpawnPt(
                    pistolEnemyConfig,
                    spawnPoints[i]
                );
        }
    }
}
