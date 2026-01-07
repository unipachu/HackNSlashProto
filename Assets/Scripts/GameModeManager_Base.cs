using UnityEngine;

/// <summary>
/// GameManager handles the changing of game modes. The game modes are managed by classes that inherit from this.
/// There can only be one game mode at a time.
/// </summary>
// TODO: Initialize and Exit methods might be useless since you could simply use Awake/Start and OnDestroy methods.
// TODO CONTD: Oh well, at least the abstract methods force you to implement them.
public abstract class GameModeManager_Base : MonoBehaviour
{
    /// <summary>
    /// Called by GameManager when starting/changing a game mode.
    /// </summary>
    public abstract void InitializeGameMode();

    /// <summary>
    /// Called by the game manager just before initializing a new game mode.
    /// </summary>
    public abstract void ExitGameMode();
}
