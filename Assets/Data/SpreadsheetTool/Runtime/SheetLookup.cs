using System.Collections.Generic;
using UnityEngine;

public class SheetLookup<T> where T : class, ISheetRowWithId
{
    private Dictionary<string, T> lookup;

    public SheetLookup(IReadOnlyList<T> rows)
    {
        Rebuild(rows);
    }

    public void Rebuild(IReadOnlyList<T> rows)
    {
        lookup = BuildById(rows);
    }

    // TODO: This is not needed.
    public T Get(string id)
    {
        if (lookup.TryGetValue(id, out T row))
            return row;

        Debug.LogError($"Row with Id '{id}' was not found.");
        return null;
    }

    public bool TryGet(string id, out T row)
    {
        return lookup.TryGetValue(id, out row);
    }

    /// <returns>Dictionary of rows indexed by each row's Id (which should be unique).</returns>
    public static Dictionary<string, T> BuildById(IReadOnlyList<T> rows)
    {
        Dictionary<string, T> lookup = new();

        if (rows == null)
            return lookup;

        for (int i = 0; i < rows.Count; i++)
        {
            T row = rows[i];

            if (row == null)
            {
                Debug.LogWarning($"Row {i} is null.");
                continue;
            }

            string id = row.Id;

            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogWarning($"Row {i} has an empty Id.");
                continue;
            }

            if (lookup.ContainsKey(id))
            {
                Debug.LogError($"Duplicate Id '{id}' found. Row {i} will be ignored.");
                continue;
            }

            lookup.Add(id, row);
        }

        return lookup;
    }

    //public static void EnsureLookupBuilt<T>(ref Dictionary<string, T> lookup, IReadOnlyList<T> rows) where T : ISheetRowWithId
    //{
    //    if (lookup == null) lookup = BuildById(rows);
    //}
}
