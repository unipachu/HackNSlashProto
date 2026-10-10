using System;
using UnityEngine;

[Serializable]
public class DbRow_EnemyWaveConfig : ISheetRowWithId{
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;
    public EnemyWave[] waves;

    public string Id => id;
}
