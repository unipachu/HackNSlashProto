using UnityEngine;

/// <summary>
/// Options menu-like settings of the game.
/// NOTE: These are the settings when game is started and are not necessarily applied if changed during runtime.
/// </summary>
public class GameSettings : Singleton<GameSettings>{
    [Header("Input")]
    [Tooltip("Mouse (or joystick hatswitch) sensitivity for look input.")]
    public float lookPointerSensitivity = 0.001f;
    [Tooltip("Squared deadzone for the left stick.")]
    public float movInputSqrDeadzone = 0.2f;

    [Header("Display and Graphics")]
    [Tooltip("-1 = content is rendered unsynchronized as fast as possible\n" +
        "NOTE: targetFrameRate is ignored if vSyncCount > 0 (only in Builds it seems).\n" +
        "NOTE 2: In the Editor, this only affects the Game view (if VSync box is not ticked in the dropdown.)")]
    public int targetFrameRate = -1;
    [Tooltip("NOTE: Doesn't seem to do anything in the Editor.")]
    public int vSyncCount = 1;
}
