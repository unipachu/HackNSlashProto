using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes utility functions for sheet lookups.
/// </summary>
public static class SheetLookup
{
    /// <returns>Dictionary of rows indexed by each row's Id (which should be unique).</returns>
    public static Dictionary<string, T> BuildById<T>(IReadOnlyList<T> rows) where T : ISheetRowWithId
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
}
