using System.Collections;
using UnityEngine;

public class EnemySpawnPt : MonoBehaviour {
    [SerializeField] Renderer sphereRenderer;

    public bool IsSpawning { get; private set; }

    private void Awake() {
        ShaderUtils.SetColor(sphereRenderer, Color.lightGray);
        sphereRenderer.enabled = false;
    }

    public void EndSpawning(AiCtrlHandle spawnedEnemy) {
        sphereRenderer.enabled = false;
        EnemyServer.inst.OnSpawnPtFinishedSpawning(spawnedEnemy);
        IsSpawning = false;
    }

    public void QuickSpawn(So_AiCpConfig enemyConfig) {
        AiCtrlHandle aiCtrl = CpFactory.SpawnAiCpAtSpawnPt(enemyConfig, transform);
        EndSpawning(aiCtrl);
    }

    IEnumerator SpawnRoutine(So_AiCpConfig enemyConfig) {
        float t = 0;
        while(t < GlobalData.inst.enemySpawnAnimDur) {
            t += Time.deltaTime;
            ShaderUtils.SetDitherAmount(sphereRenderer, t / GlobalData.inst.enemySpawnAnimDur);
            //ShaderUtils.SetFresnelAmount(sphereRenderer, 1 - t / spawnDuration);
            yield return null;
        }
        AiCtrlHandle aiCtrl = CpFactory.SpawnAiCpAtSpawnPt(enemyConfig, transform);
        EndSpawning(aiCtrl);
    }

    public bool TryBeginSpawning(So_AiCpConfig enemyConfig) {
        if (IsSpawning)
            return false;
        IsSpawning = true;
        sphereRenderer.enabled = true;
        StartCoroutine(SpawnRoutine(enemyConfig));
        return true;
    }
}