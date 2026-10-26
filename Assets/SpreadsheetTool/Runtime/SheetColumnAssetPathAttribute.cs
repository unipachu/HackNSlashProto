// Portions of this file are based on NorskaLib.Spreadsheets.
// Original project: https://github.com/NorskaGames/NorskaLib.Spreadsheets
// Original copyright: Copyright (c) 2022 NorskaGames.
// Licensed under the MIT License

using System;

/// <summary>
/// A column containing an asset path can be marked with this so that the importer knows to run asset path specific logic.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetColumnAssetPathAttribute : Attribute{}