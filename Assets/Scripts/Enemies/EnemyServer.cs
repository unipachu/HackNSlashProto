using UnityEngine;

// TODO: Enemy wave manager basically. 
public class EnemyServer : Singleton<EnemyServer>{
    [SerializeField] So_NpcSetup hammerEnemyConfig;
    [SerializeField] So_NpcSetup pistolEnemyConfig;
    [SerializeField] Transform[] spawnPoints;

    public void SpawnEnemies() {
        for (int i = 0; i < spawnPoints.Length; i++) {
            //Debug.Log($"Spawnin enemy to spawn point: {i}");
            if (i % 2 == 0) 
                PlayerSpawner.SpawnAiCpAtSpawnPt(
                    hammerEnemyConfig.btT,
                    hammerEnemyConfig.cpPrefab,
                    spawnPoints[i]
                );
            else
                PlayerSpawner.SpawnAiCpAtSpawnPt(
                    pistolEnemyConfig.btT,
                    pistolEnemyConfig.cpPrefab,
                    spawnPoints[i]
                );
        }
    }
}
