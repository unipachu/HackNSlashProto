using UnityEngine;

/// <summary>
/// Used for testing.
/// </summary>
public class GameModeManager_TestMode : GameModeManager_Base
{
    public override void InitializeGameMode()
    {

    }

    public override void ExitGameMode()
    {
        Debug.LogError("No implementation.", this);
    }
}
