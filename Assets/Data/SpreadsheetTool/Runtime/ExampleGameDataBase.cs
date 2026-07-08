using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example data base used for testing.
/// </summary>
[CreateAssetMenu(fileName = "GameDatabase", menuName = "Game Data/Spreadsheet Database")]
public class ExampleGameDatabase : SheetContainerBase
{
    [Sheet("Items")]
    [SerializeField] private List<ExampleItemData> items;

    private Dictionary<string, ExampleItemData> itemsById;

    public IReadOnlyList<ExampleItemData> Items => items;

    // TODO: Does on enable on scriptable objects work?
    private void OnEnable()
    {
        RebuildLookups();
    }

    public override void RebuildLookups()
    {
        itemsById = SheetLookup.BuildById(items);
    }

    public ExampleItemData GetItem(string id)
    {
        base.EnsureLookupBuilt<ExampleItemData>(ref itemsById, items);

        if (itemsById.TryGetValue(id, out ExampleItemData item))
            return item;

        Debug.LogError($"Item with Id '{id}' was not found.");
        return null;
    }

    public bool TryGetItem(string id, out ExampleItemData item)
    {
        base.EnsureLookupBuilt<ExampleItemData>(ref itemsById, items);
        return itemsById.TryGetValue(id, out item);
    }

    //private void EnsureLookupsBuilt()
    //{
    //    if (itemsById == null)
    //        RebuildLookups();
    //}
}

// TODO: This seems to have code that will be reused. Create class like this:

//public class SheetLookup<T>
//    where T : ISheetRowWithId
//{
//    private Dictionary<string, T> lookup;

//    public void Rebuild(IReadOnlyList<T> rows)
//    {
//        lookup = SheetLookup.BuildById(rows);
//    }

//    public T Get(string id)
//    {
//        EnsureBuilt();

//        return lookup[id];
//    }

//    public bool TryGet(string id, out T value)
//    {
//        EnsureBuilt();

//        return lookup.TryGetValue(id, out value);
//    }

//    private void EnsureBuilt()
//    {
//        if (lookup == null)
//            throw new InvalidOperationException(
//                "Lookup has not been built.");
//    }
//}