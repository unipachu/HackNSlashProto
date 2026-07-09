using UnityEngine;

/// <summary>
/// Represents one imported spreadsheet. Can have multiple "sheets".
/// Custom spreadsheet-based databases should inherit from this class.
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