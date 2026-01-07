using UnityEngine;

/// <summary>
/// Handles game mode changes.
/// </summary>
// TODO: Make this a persistent singleton.
public class GameManager : MonoBehaviour
{
    [Tooltip("Drag an game object prefab here which contains the GameModeManager_TestMode. " +
        "When starting this game mode, the game manager will instantiate that game object as its child and save the " +
        "reference to the component to the CurrentGameMode property. Yes, if you try to instantiate a reference to a " +
        "component of a prefab, it instantiates the prefab instead but returns a reference to the component.")]
    [SerializeField] private GameModeManager_TestMode _gameMode_TestModePrefab;

    public GameModeManager_Base CurrentGameMode { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnterGameMode(_gameMode_TestModePrefab);
    }
    
    /// <summary>
    /// Exits and destorys the current game mode and instantiates and initializes the newGameMode.
    /// </summary>
    public void EnterGameMode(GameModeManager_Base newGameMode)
    {
        if (CurrentGameMode != null)
        {
            CurrentGameMode.ExitGameMode();
            Destroy(CurrentGameMode);
        }
        CurrentGameMode = Instantiate(newGameMode, transform);
        CurrentGameMode.InitializeGameMode();
    }
}
