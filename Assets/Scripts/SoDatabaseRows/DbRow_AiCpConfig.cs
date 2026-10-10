using System;
using UnityEngine;

[Serializable]
public class DbRow_AiCpConfig : ISheetRowWithId {
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;
    public float aggroRange = 20;
    public float atkRange = 3;
    [SheetInspectorOnly]
    public InterfaceReference<ICp> cpPrefab;
    public BtT btT;

    public string Id => id;
}
