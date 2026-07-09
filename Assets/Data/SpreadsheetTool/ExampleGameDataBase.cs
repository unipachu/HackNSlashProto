using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example data base used for testing.
/// </summary>
[CreateAssetMenu(fileName = "ExampleGameDatabase", menuName = "Game Data/ExampleSpreadsheetDatabase")]
public class ExampleGameDatabase : SpreadsheetContainerBaseGeneric<ExampleItemData>
{
    [Sheet("Items")]
    [SerializeField]
    private List<ExampleItemData> items;

    [Sheet("Enemies")]
    [SerializeField]
    private List<ExampleEnemyData> enemies;
}