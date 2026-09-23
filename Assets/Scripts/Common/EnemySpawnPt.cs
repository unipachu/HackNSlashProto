using System.Collections;
using UnityEngine;

public class EnemySpawnPt : MonoBehaviour {
    [SerializeField] Renderer sphereRenderer;

    float spawnDuration = 2; // TODO: This should be a global variable.

    public bool IsSpawning { get; private set; }

    private void Awake() {
         // TODO: These should probably be global values too. Or reveal them in the inspector or something.
        ShaderUtils.SetDitherAmount(sphereRenderer, 0);
        ShaderUtils.SetColor(sphereRenderer, Color.lightGray);
        ShaderUtils.SetFresnelAmount(sphereRenderer, 0);
        sphereRenderer.enabled = false;
    }

    public bool TryBeginSpawning(So_AiCpConfig enemyConfig) {
        if (IsSpawning)
            return false;
        IsSpawning = true;
        sphereRenderer.enabled = true;
        StartCoroutine(SpawnRoutine(enemyConfig));
        return true;
    }

    public void EndSpawning(AiCtrlHandle spawnedEnemy) {
        sphereRenderer.enabled = false;
        EnemyServer.inst.OnSpawnPtFinishedSpawning(spawnedEnemy);
        IsSpawning = false;
    }

    IEnumerator SpawnRoutine(So_AiCpConfig enemyConfig) {
        float t = 0;
        while(t < spawnDuration) {
            t += Time.deltaTime;
            ShaderUtils.SetDitherAmount(sphereRenderer, t / spawnDuration);
            //ShaderUtils.SetFresnelAmount(sphereRenderer, 1 - t / spawnDuration);
            yield return null;
        }
        AiCtrlHandle aiCtrl = CpFactory.SpawnAiCpAtSpawnPt(enemyConfig, transform);
        EndSpawning(aiCtrl);
    }

    public void QuickSpawn(So_AiCpConfig enemyConfig) {
        AiCtrlHandle aiCtrl = CpFactory.SpawnAiCpAtSpawnPt(enemyConfig, transform);
        EndSpawning(aiCtrl);
    }
}