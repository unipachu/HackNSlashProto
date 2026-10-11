using System;
using UnityEngine;

[Serializable]
public class DbRow_HitEffectsConfig : ISheetRowWithId {
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;
    public int dmg;
    public HitT hitT;
    public KnockbackT knockbackT;
    public float knockbackStr;

    public string Id => id;
}
