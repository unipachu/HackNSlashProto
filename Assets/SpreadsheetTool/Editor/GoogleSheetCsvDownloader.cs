#if UNITY_EDITOR
using System;
using System.Net;

/// <summary>
/// Utility class for downloading .cvs file from Google Sheets.
/// </summary>
public static class GoogleSheetCsvDownloader
{
    /// <summary>
    /// Downloads a .csv from Google Sheets.
    /// </summary>
    /// <param name="spreadsheetId">In the spreadsheet URL: https://docs.google.com/spreadsheets/d/[this part here is the id]/edit?gid=0#gid=0</param>
    /// <param name="sheetName">Name of the sheet (tab) of the spreadsheet we want to download.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string DownloadCsv(string spreadsheetId, string sheetName)
    {
        if (string.IsNullOrWhiteSpace(spreadsheetId))
            throw new ArgumentException("Spreadsheet ID is empty.");

        if (string.IsNullOrWhiteSpace(sheetName))
            throw new ArgumentException("Sheet name is empty.");

        // Turns the sheet name into URL form.
        string escapedSheetName = Uri.EscapeDataString(sheetName);

        string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&sheet={escapedSheetName}";

        // .NET class which allows downloading data from the internet.
        // "using" should automatically dispose the client when we exit this scope.
        using WebClient client = new WebClient();
        // This line apparently does quite a lot, opening a secure HTTPS connection to docs.google.com, requests .csv, then converts it to a string.
        return client.DownloadString(url);
    }
}
#endif