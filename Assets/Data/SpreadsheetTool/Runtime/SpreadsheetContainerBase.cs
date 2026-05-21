using UnityEngine;

public abstract class SpreadsheetContainerBase : ScriptableObject
{
    [Header("Google Sheet")]
    [SerializeField] private string spreadsheetId;

    public string SpreadsheetId => spreadsheetId;
}