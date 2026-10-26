// Portions of this file are based on NorskaLib.Spreadsheets.
// Original project: https://github.com/NorskaGames/NorskaLib.Spreadsheets
// Original copyright: Copyright (c) 2022 NorskaGames.
// Licensed under the MIT License

/// <summary>
/// Sheet which has a column of unique ids for each row.
/// </summary>
public interface ISheetRowWithId{
    /// <summary>
    /// Row cell which is unique to each row of the sheet.
    /// </summary>
    string Id { get; }
}