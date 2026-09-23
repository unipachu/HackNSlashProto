using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveConfig_", menuName = "Scriptable Object Data/EnemyWaveConfig")]
public class So_EnemyWaveConfig : ScriptableObject {
    [Header("Waves")]
    public EnemyWave[] waves;
}
