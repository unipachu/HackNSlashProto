using UnityEngine;

/// <summary>
/// Used to store the Google Spreadsheet id, i.e. the part of the URL that points to the spreadsheet.
/// e.g. in URL: https://docs.google.com/spreadsheets/d/[this part here is the id]/edit?gid=0#gid=0
/// This is needed to download data from google sheets.
/// </summary>
public abstract class SpreadsheetContainerBase : ScriptableObject
{
    [Header("Google Sheet")]
    [Tooltip("In the spreadsheet URL: https://docs.google.com/spreadsheets/d/[this part here is the id]/edit?gid=0#gid=0")]
    [SerializeField] private string spreadsheetId;

    public string SpreadsheetId => spreadsheetId;

    protected void OnEnable()
    {
        RebuildLookups();
    }

    public abstract void RebuildLookups();

    public string GetSpreadsheetId() => spreadsheetId;
}