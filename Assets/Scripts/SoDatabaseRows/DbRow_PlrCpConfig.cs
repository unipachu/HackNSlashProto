using System;
using UnityEngine;

[Serializable]
public class DbRow_PlrCpConfig : ISheetRowWithId {
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;
    public int maxUlt = 5;

    public string Id => id;
}
