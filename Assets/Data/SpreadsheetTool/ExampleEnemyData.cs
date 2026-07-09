using System;
using UnityEngine;

/// <summary>
/// Item data used for testing. Represents a row of the Items sheet.
/// </summary>
[Serializable]
public class ExampleEnemyData : ISheetRowWithId
{
    [SheetCellRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;

    [SheetColumn("Display Name")]
    public string DisplayName;

    public int Damage;
    public int HP;
    public string UiJuma;

    public string Id => id;
}
