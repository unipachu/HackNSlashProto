using System;
using UnityEngine;

[Serializable]
public class DbRow_CpFlyingHeadConfig : ISheetRowWithId {
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;

    public float dash_ImpactDur;
    public float dash_RecoveryDur;
    public float dash_WindupDur;

    public string Id => id;
}
