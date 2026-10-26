// Portions of this file are based on NorskaLib.Spreadsheets.
// Original project: https://github.com/NorskaGames/NorskaLib.Spreadsheets
// Original copyright: Copyright (c) 2022 NorskaGames.
// Licensed under the MIT License

using System;

/// <summary>
/// If a column name is something else than the variable name (e.g. because it has symbols you don't want to use in the variable name)
/// you can optionally add this attribute to have the variable name to be different than the actual Google Spreadsheets sheet column name.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetColumnAttribute : Attribute{
    public string ColumnName { get; }

    public SheetColumnAttribute(string columnName){
        ColumnName = columnName;
    }
}
