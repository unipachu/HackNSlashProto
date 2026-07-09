using System;

/// <summary>
/// A column containing an asset path can be marked with this so that the importer knows to run asset path specific logic.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetColumnAssetPathAttribute : Attribute
{
}