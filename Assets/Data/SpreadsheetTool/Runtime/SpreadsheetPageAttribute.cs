using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SpreadsheetPageAttribute : Attribute
{
    public string PageName { get; }

    public SpreadsheetPageAttribute(string pageName)
    {
        PageName = pageName;
    }
}