using UnityEngine;

/// <summary>
/// Used for testing.
/// </summary>
public class GameModeManager_TestMode : GameModeManager_Base
{
    [SerializeField] private CSVReader _cSVReader;

    public int testValue = 0;

    public override void InitializeGameMode()
    {
        _cSVReader.ReadCSV(_cSVReader.TextAssetData);
        testValue = 8765;
    }

    public override void ExitGameMode()
    {
        Debug.LogError("No implementation.", this);
    }
}
