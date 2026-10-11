using System;
using UnityEngine;

// TODO: Currently scriptable object is still used for enemy waves.
[Serializable]
public class DbRow_EnemyWaveConfig : ISheetRowWithId{
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;
    public EnemyWave[] waves;

    public string Id => id;
}
