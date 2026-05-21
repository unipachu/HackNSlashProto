#if UNITY_EDITOR
using System;
using System.Net;
using UnityEngine;

public static class GoogleSheetCsvDownloader
{
    public static string DownloadCsv(string spreadsheetId, string sheetName)
    {
        if (string.IsNullOrWhiteSpace(spreadsheetId))
            throw new ArgumentException("Spreadsheet ID is empty.");

        if (string.IsNullOrWhiteSpace(sheetName))
            throw new ArgumentException("Sheet name is empty.");

        string escapedSheetName = Uri.EscapeDataString(sheetName);

        string url =
            $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&sheet={escapedSheetName}";

        using WebClient client = new WebClient();
        return client.DownloadString(url);
    }
}
#endif