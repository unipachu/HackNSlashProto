using UnityEngine;

/// <summary>
/// Options menu-like settings of the game.
/// </summary>
public class GameSettings : Singleton<GameSettings>{
    [Tooltip("Squared deadzone for the left stick.")]
    public float movInputSqrDeadzone = 0.2f;
    [Tooltip("Mouse (or joystick hatswitch) sensitivity for look input.")]
    public float lookPointerSensitivity = 0.001f;
}
