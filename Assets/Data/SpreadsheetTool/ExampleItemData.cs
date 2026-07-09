using System;
using UnityEngine;

/// <summary>
/// Item data used for testing. Represents a row of the Items sheet.
/// </summary>
[Serializable]
public class ExampleItemData : ISheetRowWithId
{
    [SheetCellRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;

    [SheetColumn("Display Name")]
    public string DisplayName;

    public int Damage;
    public int Price;

    public string Id => id;
}