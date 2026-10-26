// Portions of this file are based on NorskaLib.Spreadsheets.
// Original project: https://github.com/NorskaGames/NorskaLib.Spreadsheets
// Original copyright: Copyright (c) 2022 NorskaGames.
// Licensed under the MIT License

using System;

/// <summary>
/// Used to link a List of ISpreadsheetRowWithId to a spreadsheet's sheet based on the sheet name.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetAttribute : Attribute{
    public string SheetName { get; }

    public SheetAttribute(string sheetName){
        SheetName = sheetName;
    }
}