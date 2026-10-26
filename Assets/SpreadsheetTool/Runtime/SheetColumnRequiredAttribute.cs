// Portions of this file are based on NorskaLib.Spreadsheets.
// Original project: https://github.com/NorskaGames/NorskaLib.Spreadsheets
// Original copyright: Copyright (c) 2022 NorskaGames.
// Licensed under the MIT License

using System;

/// <summary>
/// Marks columns cells as required, i.e. if cell is null or white space, it will throw an error.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetColumnRequiredAttribute : Attribute{}
