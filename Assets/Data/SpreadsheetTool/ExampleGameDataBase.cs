using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example data base used for testing.
/// </summary>
// TODO: Id should be always required and it should always be the list item name because now they are two separate things.
[CreateAssetMenu(fileName = "GameDatabase", menuName = "Game Data/Spreadsheet Database")]
public class ExampleGameDatabase : SheetContainerBaseGeneric<ExampleItemData>
{
    [Sheet("Items")]
    private List<ExampleItemData> items;

    [Sheet("Enemies")]
    private List<ExampleEnemyData> enemies;
    //[SerializeField] private List<ExampleItemData> items;

    //private SheetLookup<ExampleItemData> lookup;

    //public IReadOnlyList<ExampleItemData> Items => items;


    //private void OnEnable()
    //{
    //    lookup = new SheetLookup<ExampleItemData>(items);
    //    lookup.Rebuild();
    //}
}