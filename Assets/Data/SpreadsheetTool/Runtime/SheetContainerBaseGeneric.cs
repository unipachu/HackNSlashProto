using System.Collections.Generic;
using UnityEngine;

// TODO: This can contain multiple sheets so rename it to Spreadsheet container.
public abstract class SheetContainerBaseGeneric<T> : SheetContainerBase where T : class, ISheetRowWithId
{
    [SerializeField] protected List<T> rows;

    private SheetLookup<T> lookup;

    public IReadOnlyList<T> Rows => rows;

    public override void RebuildLookups()
    {
        lookup = new SheetLookup<T>(rows);
    }

    public bool TryGet(string id, out T row)
    {
        return lookup.TryGet(id, out row);
    }
}
