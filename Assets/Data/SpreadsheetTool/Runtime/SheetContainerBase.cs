using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to store the Google Spreadsheet id, i.e. the part of the URL that points to the spreadsheet.
/// e.g. in URL: https://docs.google.com/spreadsheets/d/[this part here is the id]/edit?gid=0#gid=0
/// This is needed to download data from google sheets.
/// Inherited database classes also require a ISheetRowWithId to get the sheet data.
/// </summary>
// TODO: Should ISheetRowWithId be part of this class so that all inherited classes are required to have one? There must be better code design for this.
public abstract class SheetContainerBase : ScriptableObject
{
    [Header("Google Sheet")]
    [Tooltip("In the spreadsheet URL: https://docs.google.com/spreadsheets/d/[this part here is the id]/edit?gid=0#gid=0")]
    [SerializeField] private string spreadsheetId;

    public string SpreadsheetId => spreadsheetId;

    public abstract void RebuildLookups();

    protected void EnsureLookupBuilt<T>(ref Dictionary<string, T> lookup, IReadOnlyList<T> rows) where T : ISheetRowWithId
    {
        if (lookup == null)
            lookup = SheetLookup.BuildById(rows);
    }
}