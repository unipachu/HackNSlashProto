using System.Collections.Generic;
//using UnityEngine;

/// <summary>
/// This is the class you should inherit from when creating custom spreadsheet databases.
/// </summary>
//public abstract class SpreadsheetContainerBaseGeneric<T> : SpreadsheetContainerBase where T : class, ISheetRowWithId
//{
//    // TODO: Problem, problem, problem.
//    [SerializeField] protected List<T> rows;

//    private SheetLookup<T> lookup;

//    public IReadOnlyList<T> Rows => rows;

//    public override void RebuildLookups()
//    {
//        lookup = new SheetLookup<T>(rows);
//    }

//    public bool TryGet(string id, out T row)
//    {
//        return lookup.TryGet(id, out row);
//    }
//}
