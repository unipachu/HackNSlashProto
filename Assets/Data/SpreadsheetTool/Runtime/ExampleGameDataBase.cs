using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Game Data/Spreadsheet Database")]
public class ExampleGameDatabase : SpreadsheetContainerBase
{
    [SpreadsheetPage("Items")]
    public List<ExampleItemData> Items;
}