using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;

/// <summary>
/// Reads CVS files. NOTE: Expects rows to be separated by '\n' and cells of a row by ';'.
/// </summary>
// TODO: Make modular.
public class CSVReader : MonoBehaviour
{
    public TextAsset TextAssetData;

    [Serializable]
    // Represent the columns of the csv.
    public class TestEnemyData
    {
        public string Name;
        public int TestValue0;
        public int TestValue1;
        public float TestValue2;
        public bool TestValue3;
    }

    [Serializable]
    // Represent the rows of the csv.
    public class EnemiesListData
    {
        public List<TestEnemyData> enemies;
    }

    public EnemiesListData enemiesListData = new();

    /// <summary>
    /// Reads CSV file and saves the parameters into the enemiesListData;
    /// </summary>
    public void ReadCSV(TextAsset csv)
    {
        // Last cell in a row seems to contain a '\r' character.
        string[] rows = csv.text
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Column titles should match the names of the variables
        string[] headerRow = rows[0].Split(';');
        Debug.Assert(headerRow[0].Trim() == nameof(TestEnemyData.Name), "Name" ,this);
        Debug.Assert(headerRow[1].Trim() == nameof(TestEnemyData.TestValue0), "TestValue0", this);
        Debug.Assert(headerRow[2].Trim() == nameof(TestEnemyData.TestValue1), "TestValue1", this);
        Debug.Assert(headerRow[3].Trim() == nameof(TestEnemyData.TestValue2), "TestValue2", this);
        Debug.Assert(headerRow[4].Trim() == nameof(TestEnemyData.TestValue3), "TestValue3", this);

        Debug.Assert(rows.Length > 0, "No rows found in CSV!", this);
        
        int dataRowCount = rows.Length - 1; // minus header
        enemiesListData.enemies = new();

        // Skip row 0 (header row). 
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i]))
            {
                Debug.LogWarning("CSV reader found an empty row at index: " + i + ".", this);
                continue;
            }

            // NOTE: Expects row cells to be separated by a ';'.
            string[] rowCells = rows[i].Split(';');

            Debug.Assert(rowCells.Length > 0, "CSV found row with apparently no cells!", this);

            // Indexes of the rows skip index 0, but the indexes of the enemies do not, therefore:
            int enemyIndex = i - 1;

            // Create new enemy data and fill in its fields from the csv.
            enemiesListData.enemies.Add(new TestEnemyData());
            TryReadString(rowCells[0], out enemiesListData.enemies[enemyIndex].Name, nameof(TestEnemyData.Name));
            TryReadInt(rowCells[1], out enemiesListData.enemies[enemyIndex].TestValue0, nameof(TestEnemyData.TestValue0));
            TryReadInt(rowCells[2], out enemiesListData.enemies[enemyIndex].TestValue1, nameof(TestEnemyData.TestValue1));
            TryReadFloat(rowCells[3], out enemiesListData.enemies[enemyIndex].TestValue2, nameof(TestEnemyData.TestValue2));
            TryReadBool(rowCells[4], out enemiesListData.enemies[enemyIndex].TestValue3, nameof(TestEnemyData.TestValue3));
            // TODO: Create enum type parameter.
        }
    }

    private void TryReadString(string cell, out string output, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(cell))
            Debug.LogError("Failed reading string for " + fieldName + ".");
        output = cell;
    }

    private void TryReadInt(string cell, out int output, string fieldName)
    {
        if (!int.TryParse(cell, out output))
            Debug.LogError("Failed parsing int for " + fieldName + ".");
    }

    private void TryReadFloat(string cell, out float output, string fieldName)
    {
        if (!float.TryParse(cell, NumberStyles.Float, CultureInfo.InvariantCulture, out output))
            Debug.LogError("Failed parsing float for " + fieldName + ".");
    }

    private void TryReadBool(string cell, out bool output, string fieldName)
    {
        if (!bool.TryParse(cell, out output))
            Debug.LogError("Failed parsing bool for " + fieldName + ".");
    }
}
