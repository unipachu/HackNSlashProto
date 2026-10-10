using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example data base used for testing.
/// </summary>
[CreateAssetMenu(fileName = "ExampleGameDatabase", menuName = "Game Data/ExampleSpreadsheetDatabase")]
public class ExampleGameDatabase : SpreadsheetContainerBase{
    [Sheet("Items")]
    [SerializeField]
    List<ExampleItemData> items;

    [Sheet("Enemies")]
    [SerializeField]
    List<ExampleEnemyData> enemies;

    SheetLookup<ExampleItemData> itemLookup;
    SheetLookup<ExampleEnemyData> enemyLookup;


    public override void RebuildLookups()
    {
        itemLookup = new SheetLookup<ExampleItemData>(items);
        enemyLookup = new SheetLookup<ExampleEnemyData>(enemies);
    }


    public ExampleItemData GetItem(string id)
    {
        return itemLookup.Get(id);
    }


    public ExampleEnemyData GetEnemy(string id)
    {
        return enemyLookup.Get(id);
    }
}